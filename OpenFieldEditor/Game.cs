using System;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;

using ImGuiNET;
using OFE.EditorUI;
using OFC.Utility;
using OFC.Numerics.Random;
using OFE.Data;

namespace OFE
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
            Title = "Open Field Editor",
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
            WindowBorder = WindowBorder.Resizable,
            SrgbCapable = false,
            IsEventDriven = false,
            Flags = ContextFlags.ForwardCompatible,
        };

        //UI Temporaries
        UIConsole imguiConsole = new UIConsole(50);

        public Game() : base(gameWindowSettings, nativeWindowSettings)
        {
            ProjectFile file;
            ProjectFile.LoadFromFile("D:\\REEEE\\OpenFieldProject\\Edtior Project Example\\ProjectName.epf", out file);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            ImGuiManager.Initialize();
        }

        bool consoleEnabled = false;

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0F, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GUI
            ImGuiManager.BeginFrame();

            ImGui.DockSpaceOverViewport();

            ImGui.BeginMainMenuBar();
            if(ImGui.BeginMenu("File"))
            {
                if(ImGui.MenuItem("New")) { }

                if(ImGui.BeginMenu("Open..."))
                {
                    if (ImGui.MenuItem("From File"))
                    {

                    }

                    if(ImGui.BeginMenu("Recent Files..."))
                    {

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Save")) { }
                if(ImGui.BeginMenu("Import..."))
                {
                    if(ImGui.MenuItem("King's Field I"))
                    {

                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }


            if (ImGui.BeginMenu("View"))
            {
                if(ImGui.MenuItem("Console", "", ref consoleEnabled))
                {

                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            if (consoleEnabled)
            {
                imguiConsole.Draw();
            }

            ImGui.ShowDemoWindow();

            ImGuiManager.EndFrame();

            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            ImGuiManager.OnTextInput((uint)e.Unicode);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            ImGuiManager.Update(this, (float)e.Time);
        }
    }
}
