using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;

using OFC.Input;
using OFC.Rendering;
using OFC.Resource;
using OFC.World;
using OFC.Resource.Shader;
using OFC.Resource.Model;
using OFC.Resource.Texture;
using System.ComponentModel;
using OFC.Utility;
using System.Runtime.InteropServices;
using OFC.Numerics;
using OFC.World.Graph;
using OFC.Resource.Material;

namespace OFR
{
    class Game : GameWindow
    {
        private static readonly GameWindowSettings gameWindowSettings = new GameWindowSettings
        {
            RenderFrequency = 0d,
            UpdateFrequency = 60d
        };
        private static readonly NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            Title = "Open Field Runtime",
            Size = new OpenTK.Mathematics.Vector2i(960, 540),
            Location = new OpenTK.Mathematics.Vector2i(120, 270), //480, 270
            AutoLoadBindings = true,
            API = ContextAPI.OpenGL,
            APIVersion = Version.Parse("4.5"),
            Profile = ContextProfile.Core,
            AspectRatio = null,
            AlphaBits = 8,
            RedBits = 8,
            GreenBits = 8,
            BlueBits = 8,
            StencilBits = 0,
            DepthBits = 32,
            CurrentMonitor = Monitors.GetPrimaryMonitor().Handle,
            NumberOfSamples = 0,
            WindowState = WindowState.Normal,
            StartFocused = true,
            StartVisible = true,
            WindowBorder = WindowBorder.Fixed,
            SrgbCapable = false,
            IsEventDriven = false,
            Flags = ContextFlags.ForwardCompatible
        };

        // Very temporary testing shit
        double timeKeeper;
        int frames;

        Camera cameraTemp;


        TextureResource textureResTemp;
        TextureResource textureRes44;
        ModelResource modelResTemp;
        ModelResource modelRes2;
        ModelResource modelRes3;

        ShaderResource shaderResTemp;
        TilemapZone3D tempTilemap;
        MaterialResource dynamicMaterial;

        public Game() : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.Off;
            CursorState = CursorState.Grabbed;
        }

        protected override void OnLoad()
        {
            //
            // Testing - Sync Resources
            //
            ResourceManager.Load("Shader\\NormalTexcoord3D.jfx", out shaderResTemp);
            shaderResTemp.Use();

            ResourceManager.Load("Texture\\ga_cave_0000.tga", out textureResTemp);
            ResourceManager.Load("Model\\BARREL.ms3d", out modelRes2);

            // Initialize Debug
            GL.Enable(EnableCap.DebugOutput);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);

            //
            // Camera
            //
            cameraTemp = new Camera(60f, 960f / 540f, 0.01f, 1024f);

            //
            // Testing - Tilemap
            //
            tempTilemap = new TilemapZone3D(modelResTemp, shaderResTemp);
            tempTilemap.worldCamera = cameraTemp;
            tempTilemap.zoneGraph.Add(new ObjectGraphNode(new Vector3f(6f, 0f, 6f), Vector3f.Zero, Vector3f.Zero, modelRes2));
            tempTilemap.zoneGraph.Add(new ObjectGraphNode(new Vector3f(6f, 0f, 5f), Vector3f.Zero, Vector3f.Zero, modelRes2));

            //
            // Initialize 2D Renderer
            //
            Render2D.Initialize();
            Render2D.Resize(960, 540);

            //
            // Testing a dynamic material
            //
            

            ResourceManager.Load("Texture\\ga_test_0000.tga", out textureRes44);
            dynamicMaterial = new(shaderResTemp, EResourceState.Ready, 0, "internal", 12345678);
            dynamicMaterial.SetParameter("diffuseMap", textureRes44);

            ResourceManager.Store("testDynamic", dynamicMaterial);

            ResourceManager.Dump();

            ResourceManager.Load("Model\\plane_xz.ms3d", out modelRes3);
            modelRes3.GetMesh<StaticMesh>(0).Material = dynamicMaterial;

            ResourceManager.Load("Model\\ts_cave_0000.ms3d", out modelResTemp);
            //
            // Initialize Input Manager
            //
            InputManager.Initialize(this);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderContext.Reset();
            RenderContext.CurrentCamera = cameraTemp;

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            //These will be set by the material automatically...
            Render3D.DrawMesh(modelRes3, 0, Matrix4f.CreateTranslation(0, 0, 0));
            Render3D.DrawModel(modelResTemp, Matrix4f.CreateTranslation(0, 0, 0));

            Render3D.DrawModel(modelRes2, Matrix4f.CreateTranslation(-5, 0,  0));
            Render3D.DrawModel(modelRes2, Matrix4f.CreateTranslation( 5, 0,  0));
            Render3D.DrawModel(modelRes2, Matrix4f.CreateTranslation( 0, 0, -5));
            Render3D.DrawModel(modelRes2, Matrix4f.CreateTranslation( 0, 0,  5));

            //
            // Track FPS/ms
            //
            timeKeeper += args.Time;
            frames++;
            if(timeKeeper >= 1d)
            {
                Console.WriteLine($"Batches This Frame = {Render2D.Batches}, fps = {frames}, ms = {1000d * args.Time:F2}");
                timeKeeper = 0d;
                frames = 0;
            }

            //Cycle buffers
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Update Input Manager
            InputManager.Update();

            // Update Camera
            cameraTemp.AddRotation(InputManager.InputValue("RHAxisX") * -2.0f, InputManager.InputValue("RHAxisY") * -2.0f, 0f);

            cameraTemp.AddPosition(cameraTemp.Front * (4f * InputManager.InputValue("LHAxisY") * (float)e.Time));
            cameraTemp.AddPosition(cameraTemp.Right * (4f * InputManager.InputValue("LHAxisX") * (float)e.Time));
            cameraTemp.Update();
        }

        protected void DebugCallback(DebugSource source, DebugType type, int ID, DebugSeverity severity, int length, nint message, nint userParam)
        {
            string ErrSource = source switch
            {
                DebugSource.DebugSourceApi => "API",
                DebugSource.DebugSourceApplication => "Application",
                DebugSource.DebugSourceOther => "Other",
                DebugSource.DebugSourceShaderCompiler => "Shader Compiler",
                DebugSource.DebugSourceThirdParty => "Third Party",
                DebugSource.DebugSourceWindowSystem => "Window System",
                _ => "?"
            };

            string ErrType = type switch
            {
                DebugType.DebugTypeDeprecatedBehavior => "Deprecated",
                DebugType.DebugTypePerformance => "Performance",
                DebugType.DebugTypePortability => "Portability",
                DebugType.DebugTypeError => "Error",
                DebugType.DebugTypeUndefinedBehavior => "Undefined Behaviour",
                _ => "?"
            };

            string ErrSevr = severity switch
            {
                DebugSeverity.DebugSeverityHigh   => "High",
                DebugSeverity.DebugSeverityMedium => "Medium",
                DebugSeverity.DebugSeverityLow    => "Low",
                _ => "None"
            };

            Log.Write($"GL:{ErrType}:{ErrSevr}", 0xFFC000, Marshal.PtrToStringAnsi(message, length));
        }
    }
}
