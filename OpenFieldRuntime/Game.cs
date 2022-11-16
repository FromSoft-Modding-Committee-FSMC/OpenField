using System;
using System.Collections.Generic;
using System.Threading;

using OpenTK;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

using OFC.Utility;
using OFC.Input;
using OFC.Rendering;
using OFC.Mathematics;

using OFC.Asset;
using OFC.Asset.Factory;
using OFC.Asset.Format;
using OFC.Asset.FileFormat;

namespace OFR
{
    class Game : GameWindow
    {
        private static readonly GameWindowSettings gameWindowSettings = new GameWindowSettings
        {
            RenderFrequency = 0.0d,
            UpdateFrequency = 0.0d
        };
        private static readonly NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            Title = "Open Field Runtime",
            Size = new OpenTK.Mathematics.Vector2i(1024, 768),
            Location = new OpenTK.Mathematics.Vector2i(448, 156),
            AutoLoadBindings = true,
            API = ContextAPI.OpenGL,
            APIVersion = Version.Parse("3.3"),
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
            Flags = ContextFlags.ForwardCompatible,
        };

        public Game() : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.Off;
            InputManager.Initialize(this);
        }

        private static Texture testTexture;
        private static Texture testTexture2;
        private static Font testFont;

        protected override void OnLoad()
        {
            base.OnLoad();

            TextureFactory textureFactory = new TextureFactory();

            //Test Export Formats
            List<string> expFormats = textureFactory.EnumerateExportableFormats();
            Log.Info("Exportable Texture Formats: ");
            foreach(string fmtDesc in expFormats)
            {
                Console.WriteLine(fmtDesc);
            }
            Console.WriteLine();

            //Test Import Formats
            List<string> impFormats = textureFactory.EnumerateImportableFormats();
            Log.Info("Importable Texture Formats: ");
            foreach (string fmtDesc in impFormats)
            {
                Console.WriteLine(fmtDesc);
            }
            Console.WriteLine();

            //Test DDS IMport
            TextureAsset texture;
            if (!textureFactory.GetHandler(0).Load("Resources\\Font\\bookman3.dds", out texture))
            {
                Log.Error("Failed to load file!");
            }
            testTexture = new Texture(texture);

            if (!textureFactory.GetHandler(0).Load("Resources\\Texture\\test_rgbaf32.dds", out texture))
            {
                Log.Error("Failed to load file!");
            }
            testTexture2 = new Texture(texture);

            //Test Fonts
            testFont = new Font("Resources\\Font\\bookman.json");

            Render2D.Initialize();

            Resize += Render2D.Resize;
        }

        private double timeTotal;
        private double frameTotal;
        private int frameSampleCount;

        float rot = 0f;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0F, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector2s statPos = new(0f, 32);
            Vector2s XY = new((1024f/2f)-128f, (768f/2f)-128f);
            Vector2s WH = new(256f, 256f);
            Colour C1 = new(0f, 1f, 1f, 1f);
            Colour C2 = new(1f, 0f, 1f, 1f);
            Colour C3 = new(1f, 1f, 0f, 1f);
            Colour C4 = new(1f, 1f, 1f, 1f);

            rot += (float) (MathF.PI * args.Time);
            if(rot >= 360f)
            {
                rot = 0f;
            }

            Render2D.DrawBegin();
            for (int i = 0; i < 5000; ++i)
            {
                Render2D.DrawRectangle(ref XY, ref WH, ref C1, ref C2, ref C3, ref C4, ref testTexture);
            }

            Render2D.DrawText(ref statPos, ref testFont, ref C4, $"FPS: {(1f / args.Time):F0}, MS: {(args.Time * 1000):F4}, Batches: {Render2D.BatchesThisFrame}");
            Render2D.DrawEnd();

            //Framerate calculation
            frameTotal += (1f / args.Time);
            frameSampleCount++;

            timeTotal += args.Time;
            if(timeTotal >= 1.0d)
            {
                Console.WriteLine($"Frame [FPS: {(frameTotal / frameSampleCount):F0}, MS: {(args.Time * 1000):F4}, Batches: {Render2D.BatchesThisFrame}]");

                frameSampleCount = 0;
                frameTotal = 0f;
                timeTotal = 0d;
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            InputManager.Update();

            if(InputManager.InputReleased("TestButton"))
            {
                Close();
            }
        }
    }
}
