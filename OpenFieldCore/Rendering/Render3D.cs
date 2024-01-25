using OFC.Numerics;
using OFC.Resource.Model;
using OFC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Rendering
{
    public static class Render3D
    {
        [Flags]
        enum Render3DState : uint
        {

        }

        public static void DrawMesh(ModelResource model, int meshIndex, Matrix4f transform)
        {
            //Get mesh from model
            StaticMesh mesh = model.GetMesh<StaticMesh>(meshIndex);

            if (mesh == null)
            {
                Log.Warn($"Invalid Mesh Index! [model = {model.Source}, index = {meshIndex}]");
                return;
            }
            
            //Does the mesh have a valid material? We should make sure they do by having a default material.
            if(mesh.Material != null)
            {
                //If the context isn't currently using this materials shader, we must bind it.
                if(RenderContext.CurrentShader != mesh.Material.Shader.Hash)
                {
                    mesh.Material.Shader.Use();

                    // Set Camera Parameters
                    mesh.Material.SetParameter("view",       RenderContext.CurrentCamera.ViewMatrix);
                    mesh.Material.SetParameter("projection", RenderContext.CurrentCamera.ProjectionMatrix);

                    // Set Renderer Parameters
                    // ...
                    // ...
                }

                // Set Model Parameters
                mesh.Material.SetParameter("model", transform);

                // Set Material
                mesh.Material.Use();
            }

            //Finally, Draw the mesh
            mesh.Draw();
        }

        public static void DrawModel(ModelResource model, Matrix4f transform)
        {
            for(int i = 0; i < model.MeshCount; ++i)
                DrawMesh(model, i, transform);
        }
    }
}
