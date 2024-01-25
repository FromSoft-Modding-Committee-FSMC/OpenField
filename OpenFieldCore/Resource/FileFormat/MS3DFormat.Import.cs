using MoonSharp.Interpreter;
using OFC.IO;
using OFC.Resource.Material;
using OFC.Resource.Model;
using OFC.Resource.Shader;
using OFC.Resource.Texture;
using OFC.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OFC.Resource.FileFormat
{
    public partial class MS3DFormat
    {
        struct ImmVertex
        {
            public float PX;
            public float PY;
            public float PZ;
            //public float NX;
            //public float NY;
            //public float NZ;
            public uint N;
            public Half TU;
            public Half TV;

            public static bool operator ==(ImmVertex left, ImmVertex right)
            {
                return 
                    left.PX == right.PX && left.PY == right.PY && left.PZ == right.PZ &&
                    left.N  ==  right.N &&
                    left.TU == right.TU && left.TV == right.TV;
            }
            public static bool operator !=(ImmVertex left, ImmVertex right)
            {
                return
                    left.PX != right.PX || left.PY != right.PY || left.PZ != right.PZ ||
                    left.N  != right.N  ||
                    left.TV != right.TV || left.TV != right.TV;
            }
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        public unsafe static ModelResource LoadFromStream(InputStream ins, ref ModelResource result, in object parameters)
        {
            //Read MS3D Header
            SMS3DHeader header = new()
            {
                tag = ins.ReadFixedString(10),
                version = ins.ReadS32()
            };

            // Read MS3D Vertices
            ushort numVertex = ins.ReadU16();
            SMS3DVertex[] vertices = new SMS3DVertex[numVertex];
            for(int i = 0; i < numVertex; ++i)
            {
                vertices[i] = new SMS3DVertex
                {
                    flags = ins.ReadU8(),
                    xyz = new float[3] { ins.ReadF32(), ins.ReadF32(), ins.ReadF32() },
                    boneID = ins.ReadS8(),
                    refCount = ins.ReadU8()
                };
            }

            // Read MS3D Triangles
            ushort numTriangle = ins.ReadU16();
            SMS3DTriangle[] triangles = new SMS3DTriangle[numTriangle];
            for (int i = 0; i < numTriangle; ++i)
            {
                triangles[i] = new SMS3DTriangle
                {
                    flags = ins.ReadU16(),
                    vIndices = new ushort[3] { ins.ReadU16(), ins.ReadU16(), ins.ReadU16() },
                    vNormal = new float[9]
                    {
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(),
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(),
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32()
                    },
                    vTexU = new float[3] { ins.ReadF32(), ins.ReadF32(), ins.ReadF32() },
                    vTexV = new float[3] { ins.ReadF32(), ins.ReadF32(), ins.ReadF32() },
                    smoothingGroup = ins.ReadU8(),
                    groupID = ins.ReadU8()
                };
            }

            // Read MS3D Groups
            ushort numGroup = ins.ReadU16();
            SMS3DGroup[] groups = new SMS3DGroup[numGroup];
            for(int i = 0; i < numGroup; ++i)
            {
                groups[i] = new SMS3DGroup
                {
                    flags = ins.ReadU8(),
                    name  = ins.ReadFixedString(32),
                    numTriangle = ins.ReadU16()
                };
                groups[i].tIndices = ins.ReadU16s(groups[i].numTriangle);
                groups[i].materialID = ins.ReadS8();
            }

            // Read MS3D Materials
            ushort numMaterial = ins.ReadU16();
            SMS3DMaterial[] materials = new SMS3DMaterial[numMaterial];
            for(int i = 0; i < numMaterial; ++i)
            {
                materials[i] = new SMS3DMaterial
                {
                    name = ins.ReadFixedString(32),
                    ambientRGBX = new float[]
                    {
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(), ins.ReadF32()
                    },
                    diffuseRGBX = new float[]
                    {
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(), ins.ReadF32()
                    },
                    specularRGBX = new float[]
                    {
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(), ins.ReadF32()
                    },
                    emissiveRGBX = new float[]
                    {
                        ins.ReadF32(), ins.ReadF32(), ins.ReadF32(), ins.ReadF32()
                    },
                    specularPower = ins.ReadF32(),
                    transparency = ins.ReadF32(),
                    mode = ins.ReadS8(),
                    diffuseMapFName = ins.ReadFixedString(128),
                    alphaMapFName   = ins.ReadFixedString(128)
                };
            }

            //
            // Prepare Resource
            //
            result.Reserve(numGroup, 1, numMaterial);

            //Construct & Load MS3D Vertex Format
            VertexFormat ms3dFormat = new()
            {
                components = new SVertexComponent[]
                {
                    new SVertexComponent    //Position
                    {
                        type     = EVertexComponentType.F32,
                        count    = 3,
                        streamID = 0,
                        offset   = 0,
                        size     = 12
                    },
                    new SVertexComponent    //Normal
                    {
                        type     = EVertexComponentType.S1010102,
                        count    = 4,
                        streamID = 0,
                        offset   = 12,
                        size     = 4
                    },
                    new SVertexComponent    //Texcoord
                    {
                        type     = EVertexComponentType.F16,
                        count    = 2,
                        streamID = 0,
                        offset   = 16,
                        size     = 4
                    }
                },
                size = 20,
            };

            // Build & Load each MS3D Mesh
            //
            // Notes:
            // - MS3D meshes are not stored as +X, +Y, -Z (?), but -X, +Y, -Z (?)
            // - MS3D meshes are indexed for file size, not rendering.
            //

            // First convert all materials
            foreach (SMS3DMaterial ms3dMat in materials)
            {
                MaterialResource basicMaterial;

                // Does this material already exist?
                if(ResourceManager.Get(new string(ms3dMat.name).Split('\0')[0], out basicMaterial))
                {
                    result.LoadMaterial(basicMaterial);

                    Log.Info($"MS3D Material already exists! [name = {new string(ms3dMat.name)}]");
                    continue;
                }

                Log.Warn($"MS3D Material does not exist! [name = {new string(ms3dMat.name)}]");

                // It doesn't. Create.
                ResourceManager.Load($"Shader\\NormalTexcoord3D.jfx", out ShaderResource baseShader);

                ResourceManager.Load($"Texture\\{new string(ms3dMat.diffuseMapFName).Split('\0')[0]}", out TextureResource diffuseMap);


                basicMaterial = new MaterialResource(baseShader, EResourceState.Ready, 0, result.Source, 69420422);
                basicMaterial.SetParameter("diffuseMap", diffuseMap);

                result.LoadMaterial(basicMaterial);

                // Store
                ResourceManager.Store($"{new string(ms3dMat.name)}", basicMaterial);
            }

            foreach (SMS3DGroup group in groups)
            {
                //Allocation of buffers
                byte[] vbuffer = new byte[(3 * group.numTriangle) * ms3dFormat.size];
                int currentVtx = 0;

                byte[] ibuffer = new byte[(3 * group.numTriangle) * 2];
                int currentInd = 0;

                //Fill vertex buffer
                fixed(void* vbp = vbuffer)
                {
                    ImmVertex* vb = (ImmVertex*)vbp;

                    foreach(ushort triIndex in group.tIndices)
                    {
                        SMS3DTriangle triangle = triangles[triIndex];

                        //Build Vertex Buffer
                        SMS3DVertex oldVertex;
                        ImmVertex newVertex;

                        static uint NtoCN(float X, float Y, float Z)
                        {
                            uint XS = (uint)(X < 0 ? 1 : 0);
                            uint YS = (uint)(Y < 0 ? 1 : 0);
                            uint ZS = (uint)(Z < 0 ? 1 : 0);

                            return
                                (ZS << 29) | (((uint)(Z * 511f + (ZS << 9)) & 0x3FF) << 20) |
                                (YS << 19) | (((uint)(Y * 511f + (YS << 9)) & 0x3FF) << 10) |
                                (XS << 09) | (((uint)(X * 511f + (XS << 9)) & 0x3FF) << 00);
                        }

                        for (int i = 0; i < 3; ++i)
                        {
                            //Construct new vertex
                            oldVertex = vertices[triangle.vIndices[i]];
                            newVertex = new ImmVertex
                            {
                                // Position Copy
                                PX = -oldVertex.xyz[0],
                                PY = oldVertex.xyz[1],
                                PZ = oldVertex.xyz[2],

                                // Compressed Normal Copy
                                N = NtoCN(-triangle.vNormal[(3 * i) + 0], triangle.vNormal[(3 * i) + 1], triangle.vNormal[(3 * i) + 2]),

                                // Compressed Texcoord Copy
                                TU = (Half)triangle.vTexU[i],
                                TV = (Half)triangle.vTexV[i]
                            };

                            //Is this vertex already part of the vbuffer
                            int vertexID = 0;
                            while(vertexID < currentVtx)
                            {
                                if (vb[vertexID] == newVertex)
                                    break;
                                vertexID++;
                            }

                            //If this condition passes, it's because we exhausted the VB in the prior loop.
                            if (vertexID == currentVtx)
                                vb[currentVtx++] = newVertex;   //So add the new vertex to our list.

                            //Write to the index buffer
                            fixed (void* ibp = ibuffer)
                                ((ushort*)ibp)[currentInd++] = (ushort)vertexID;
                        }
                    }
                }

                //Store the newly constructed mesh inside the result.
                StaticMesh groupMesh = new()
                {
                    Format   = ms3dFormat
                };

                MaterialResource m = null;
                if(group.materialID >= 0)
                    ResourceManager.Get($"{new string(materials[group.materialID].name)}", out m);
                groupMesh.Material = m;

                groupMesh.LoadVertices(ref vbuffer);
                groupMesh.LoadIndices(ref ibuffer, EMeshIndexType.UInt16, currentInd);
                result.LoadMesh(groupMesh);
            }

            return result;
        }
    }
}
