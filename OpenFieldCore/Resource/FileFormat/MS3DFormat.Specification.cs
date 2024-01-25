using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.FileFormat
{
    public partial class MS3DFormat
    {
        private const int MaxVertexCount = 65534;
        private const int MaxTriangleCount = 65534;
        private const int MaxGroups = 255;
        private const int MaxMaterials = 128;
        private const int MaxJoints = 128;

        [Flags]
        enum EMS3DVertexFlags : byte
        {
            Selected = 1,
            Hidden = 2,
            Selected2 = 4,
            Dirty = 8
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SMS3DHeader
        {
            public char[] tag;      //MS3D Format Tag. Should be 'MS3D000000'
            public int version;     //MS3D Format Version. Should be 4.
        }

        //word numVertex;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SMS3DVertex
        {
            public byte flags;              //Selected | Selected2 | Hidden
            public float[] xyz;             //Vertex Position. 3 Components.
            public sbyte boneID;            //Bone Index (-1, or 0 to 128. -1 = Unused)
            public byte refCount;           //Number of times this vertex is used
        }
        
        //word numTriangle;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SMS3DTriangle
        {
            public ushort flags;            //Selected | Selected2 | Hidden
            public ushort[] vIndices;       //Vertex Indices. 3 Components.
            public float[] vNormal;         //Vertex Normal. 9 Components. XYZ/XYZ/XYZ
            public float[] vTexU;           //Vertex Texcoord U. 3 Components. U/U/U
            public float[] vTexV;           //Vertex Texcoord V. 3 Components. V/V/V
            public byte smoothingGroup;     //Normal smoothing group (no idea what this actually does);
            public byte groupID;            //The ID of the group this triangle belongs to (pointless.)
        }

        //word numGroup;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SMS3DGroup
        {
            public byte flags;              //Selected | Hidden
            public char[] name;             //32 Bytes. Group Name.
            public ushort numTriangle;      //Number of triangle indices that follow.
            public ushort[] tIndices;       //Array of triangle indices. Equal to 'numTriangle' in length.
            public sbyte materialID;        //The ID of the material this group uses.
        }

        //word numMaterial;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SMS3DMaterial
        {
            public char[] name;             //32 Bytes. Material Name.
            public float[] ambientRGBX;     //4 Entries. Ambient Tone
            public float[] diffuseRGBX;     //4 Entries. Diffuse Tone
            public float[] specularRGBX;    //4 Entries. Specular Tone
            public float[] emissiveRGBX;    //4 Entries. Emissive Tone
            public float specularPower;     //Specular Power Multiplier
            public float transparency;      //Overall transparency of the material
            public sbyte mode;              //I have no fucking idea.
            public char[] diffuseMapFName;  //Diffuse Map File Name. 128 bytes.
            public char[] alphaMapFName;    //Alpha Mask Map File Name. 128 bytes.
        }
    }
}
