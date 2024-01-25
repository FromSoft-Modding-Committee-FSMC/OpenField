using System.Diagnostics;
using System;

using OFC.World.Tilemap;
using OFC.Rendering;
using OFC.Utility;
using OFC.Resource.Shader;
using OFC.Resource.Model;
using OFC.World.Graph;
using System.Collections.Generic;

namespace OFC.World
{
    public class TilemapZone3D : IZone
    {
        Stopwatch renderTimer = new Stopwatch();

        // Parent Reference (IWorld)
        public Camera worldCamera;

        // Tileset Data
        ShaderResource tilesetShader;
        ModelResource  tileset;

        // Tilemap Data
        TilemapLayer3D[] layers;
        int currentLayer;

        // SceneGraph
        public List<IGraphNode> zoneGraph = new List<IGraphNode>();

        public TilemapZone3D(ModelResource tileset, ShaderResource tilesetShader)
        {
            //Store Reference to Tileset
            this.tileset = tileset;
            this.tilesetShader = tilesetShader;

            // DEBUG
            layers = new TilemapLayer3D[1];

            int w = 8, h = 8;

            STile3D[,] tilemap = new STile3D[h, w];

            //Fill Floor
            for(int y = 0; y < h; ++y)
            {
                for(int x = 0; x < w; ++x)
                {
                    tilemap[y, x] = new STile3D
                    {
                        flags = 1 | (0 << 8),
                        meshID = 0,
                        elevation = 0,
                        padding = 0
                    };
                }
            }

            //Fill Walls
            for(int i = 1; i < h-1; ++i)
            {
                tilemap[i, 0] = new STile3D
                {
                    flags = 1 | (0 << 8),
                    meshID = (ushort)(1 + (i % 2)),
                    elevation = 0,
                    padding = 0
                };

                tilemap[i, w - 1] = new STile3D
                {
                    flags = 1 | (2 << 8),
                    meshID = (ushort)(1 + (i % 2)),
                    elevation = 0,
                    padding = 0
                };

                tilemap[0, i] = new STile3D
                {
                    flags = 1 | (1 << 8),
                    meshID = (ushort)(1 + (i % 2)),
                    elevation = 0,
                    padding = 0
                };

                tilemap[h - 1, i] = new STile3D
                {
                    flags = 1 | (3 << 8),
                    meshID = (ushort)(1 + (i % 2)),
                    elevation = 0,
                    padding = 0
                };
            }

            //Fill Corners
            tilemap[0, 0] = new STile3D
            {
                flags = 1 | (0 << 8),
                meshID = 3,
                elevation = 0,
                padding = 0
            };
            tilemap[h - 1, 0] = new STile3D
            {
                flags = 1 | (3 << 8),
                meshID = 3,
                elevation = 0,
                padding = 0
            };
            tilemap[0, w - 1] = new STile3D
            {
                flags = 1 | (1 << 8),
                meshID = 3,
                elevation = 0,
                padding = 0
            };
            tilemap[h - 1, w -1] = new STile3D
            {
                flags = 1 | (2 << 8),
                meshID = 3,
                elevation = 0,
                padding = 0
            };

            //Push it real good.
            TilemapLayer3D test = new TilemapLayer3D(tilemap, w, h);

            layers[0] = test;
        }

        public void OnRenderBegin()
        {
            renderTimer.Restart();
        }

        public void OnRenderEnd()
        {
            renderTimer.Stop();
        }

        public void OnRender()
        {
            int baseX = (int)Math.Clamp(Math.Floor(worldCamera.Position.X + 1), 0, layers[currentLayer].Width-1);
            int baseZ = (int)Math.Clamp(Math.Floor(worldCamera.Position.Z), 0, layers[currentLayer].Height-1);
            int tileX, tileZ;

            const int mapRad = 13;
            int mapNow = 1;

            TilemapLayer3D layer;

            int layerN = 0;
            while (layerN < layers.Length)
            {
                if ((layers[currentLayer][baseX, baseZ].flags >> layerN) == 0)
                {
                    layerN++;
                    continue;
                }

                layer = layers[layerN];

                //
                // Render Center Tile
                //
                tilesetShader.UniformMatrix4f("model", ref layer.TileTransforms[baseZ, baseX]);
                tileset.Draw(layer[baseX, baseZ].meshID);

                //
                // Render Surrounding Tiles (Front -> Back)
                //
                while (mapNow <= mapRad)
                {
                    for (int i = 0; i < (mapNow << 1); ++i)
                    {
                        //Top
                        tileX = (baseX - mapNow) + i;
                        tileZ = (baseZ + mapNow);

                        if (tileX >= 0 && tileZ >= 0 && tileX < layer.Width && tileZ < layer.Height)
                        {
                            tilesetShader.UniformMatrix4f("model", ref layer.TileTransforms[tileZ, tileX]);
                            tileset.Draw(layer[tileX, tileZ].meshID);
                        }

                        //Left
                        tileX = (baseX + mapNow);
                        tileZ = (baseZ + mapNow) - i;

                        if (tileX >= 0 && tileZ >= 0 && tileX < layer.Width && tileZ < layer.Height)
                        {
                            tilesetShader.UniformMatrix4f("model", ref layer.TileTransforms[tileZ, tileX]);
                            tileset.Draw(layer[tileX, tileZ].meshID);
                        }

                        //Bottom
                        tileX = (baseX + mapNow) - i;
                        tileZ = (baseZ - mapNow);

                        if (tileX >= 0 && tileZ >= 0 && tileX < layer.Width && tileZ < layer.Height)
                        {
                            tilesetShader.UniformMatrix4f("model", ref layer.TileTransforms[tileZ, tileX]);
                            tileset.Draw(layer[tileX, tileZ].meshID);
                        }

                        //Right
                        tileX = (baseX - mapNow);
                        tileZ = (baseZ - mapNow) + i;

                        if (tileX >= 0 && tileZ >= 0 && tileX < layer.Width && tileZ < layer.Height)
                        {
                            tilesetShader.UniformMatrix4f("model", ref layer.TileTransforms[tileZ, tileX]);
                            tileset.Draw(layer[tileX, tileZ].meshID);
                        }
                    }
                    mapNow++;
                }

                layerN++;
            }

            foreach(IGraphNode node in zoneGraph)
            {
                //Skip Disabled Nodes (maybe add DrawEnabled/UpdateEnabled flags?..)
                if ((node.CurrentState & EGraphNodeState.Enabled) < 0)
                    continue;

                //tilesetShader.UniformMatrix4f("model", ref node.Transform);

                //Draw the base node
                //node.Draw();

            }
        }
    }
}
