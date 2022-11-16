using ImGuiNET;
using OFC.Mathematics;
using OFC.Rendering;
using OFC.Utility;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;


using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

namespace OFE.EditorUI
{
    public static class ImGuiManager
    {
        //Data
        private static Shader shader;
        private static Matrix44 camera;

        private static int vao;
        private static int vbo;
        private static int ebo;
        private static int maxVertexCount = 1000;
        private static int maxIndexCount  = 100;
        private static int fontTexture;

        private static IntPtr context;
        private static ImGuiIOPtr imIO;
        private static ImGuiMouseCursor lastCursor;

        private static GLState glState = new GLState();

        private static int windowWidth = 0;
        private static int windowHeight = 0;

        struct GLState
        {
            int activeTexture;
            int textureId;
            int programId;
            int samplerId;
            int vaoId;
            int blendSrcRGB;
            int blendSrcA;
            int blendDstRGB;
            int blendDstA;
            int blendEquationRGB;
            int blendEquationA;

            int[] polygonMode;
            int[] viewport;
            int[] scissorBox;

            bool blendEnabled;
            bool cullfaceEnabled;
            bool depthtestEnabled;
            bool stenciltestEnabled;
            bool scissortestEnabled;

            public void GetState()
            {
                activeTexture = GL.GetInteger(GetPName.ActiveTexture);
                textureId = GL.GetInteger(GetPName.Texture2D);
                programId = GL.GetInteger(GetPName.CurrentProgram);
                samplerId = GL.GetInteger(GetPName.SamplerBinding);
                vaoId = GL.GetInteger(GetPName.VertexArrayBinding);

                blendSrcRGB = GL.GetInteger(GetPName.BlendSrcRgb);
                blendSrcA = GL.GetInteger(GetPName.BlendSrcAlpha);
                blendDstRGB = GL.GetInteger(GetPName.BlendDstRgb);
                blendDstA = GL.GetInteger(GetPName.BlendDstAlpha);
                blendEquationRGB = GL.GetInteger(GetPName.BlendEquationRgb);
                blendEquationA = GL.GetInteger(GetPName.BlendEquationAlpha);

                polygonMode = new int[2];
                GL.GetInteger(GetPName.PolygonMode, polygonMode);

                scissorBox = new int[4];
                GL.GetInteger(GetPName.ScissorBox, scissorBox);

                viewport = new int[4];
                GL.GetInteger(GetPName.Viewport, viewport);

                blendEnabled = GL.IsEnabled(EnableCap.Blend);
                cullfaceEnabled = GL.IsEnabled(EnableCap.CullFace);
                depthtestEnabled = GL.IsEnabled(EnableCap.DepthTest);
                stenciltestEnabled = GL.IsEnabled(EnableCap.StencilTest);
                scissortestEnabled = GL.IsEnabled(EnableCap.ScissorTest);
            }

            public void SetState()
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendEquation(BlendEquationMode.FuncAdd);
                GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.StencilTest);
                GL.Enable(EnableCap.ScissorTest);
            }

            public void RestoreState()
            {
                if (blendEnabled) { GL.Enable(EnableCap.Blend); } else { GL.Disable(EnableCap.Blend); }
                if (cullfaceEnabled) { GL.Enable(EnableCap.CullFace); }else { GL.Disable(EnableCap.CullFace); }
                if (depthtestEnabled) { GL.Enable(EnableCap.DepthTest); } else { GL.Disable(EnableCap.DepthTest); }
                if (stenciltestEnabled) { GL.Enable(EnableCap.StencilTest); } else { GL.Disable(EnableCap.StencilTest); }
                if (scissortestEnabled) { GL.Enable(EnableCap.ScissorTest); } else { GL.Disable(EnableCap.ScissorTest); }

                GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
                GL.PolygonMode((MaterialFace)polygonMode[0], (PolygonMode)polygonMode[1]);
                GL.Scissor(scissorBox[0], scissorBox[1], scissorBox[2], scissorBox[3]);

                GL.BlendEquationSeparate((BlendEquationMode)blendEquationRGB, (BlendEquationMode)blendEquationA);
                GL.BlendFuncSeparate((BlendingFactorSrc)blendSrcRGB, (BlendingFactorDest)blendDstRGB, (BlendingFactorSrc)blendSrcA, (BlendingFactorDest)blendDstA);

                GL.BindVertexArray(vaoId);
                GL.BindSampler(0, samplerId);
                GL.UseProgram(programId);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.ActiveTexture((TextureUnit)activeTexture);
            } 
        }

        public static void Initialize()
        {
            //Initialize ImGUI Context, default state..
            context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            imIO = ImGui.GetIO();
            imIO.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            imIO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            ImGui.StyleColorsLight();

            //CustomStyle();

            //ImGUI Key Mapping
            imIO.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            imIO.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            imIO.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            imIO.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            imIO.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            imIO.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            imIO.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            imIO.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            imIO.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            imIO.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            imIO.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
            imIO.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            imIO.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            imIO.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            imIO.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            imIO.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            imIO.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            imIO.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            imIO.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;

            //Initialize Shader
            shader = new Shader("Resources\\Shader\\ImGUI.vss", "Resources\\Shader\\ImGUI.fss", new string[] { "camera" }, new string[] { "fontTexture" });
            shader.SetSamplerWrapMode("fontTexture", SamplerWrapMode.Repeat, SamplerWrapMode.Repeat);
            shader.SetSamplerMagFilter("fontTexture", SamplerFilterMode.Linear);
            shader.SetSamplerMinFilter("fontTexture", SamplerFilterMode.Linear);

            //Initialize OpenGL Buffers
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 20 * maxVertexCount, IntPtr.Zero, BufferUsageHint.StreamDraw);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 2 * maxIndexCount, IntPtr.Zero, BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 20, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 20, 8);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, false, 20, 16);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);

            //Initialise ImGUI Font
            imIO.Fonts.AddFontDefault();
            imIO.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

            fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, fontTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            imIO.Fonts.SetTexID(fontTexture);
            imIO.Fonts.ClearTexData();

            Log.Info("ImGUI.NET Successfully Initialized!");
        }
        
        private static void CustomStyle()
        {
            ImGuiStylePtr style = ImGui.GetStyle();

            style.FrameRounding = 0f;

            style.Colors[(int)ImGuiCol.Header] = new Colour(230, 126, 34, 255).AsNumericsVector;
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Colour(235, 152, 78, 255).AsNumericsVector;
        }

        public static void Update(GameWindow window, float deltaTime)
        {
            //Update Window
            imIO.DisplaySize = new System.Numerics.Vector2(window.ClientSize.X, window.ClientSize.Y);
            imIO.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);
            imIO.DeltaTime = deltaTime;

            //Update ImGUI Mouse.
            MouseState mouseState = window.MouseState;

            imIO.MouseDown[0] = mouseState[MouseButton.Left];
            imIO.MouseDown[1] = mouseState[MouseButton.Right];
            imIO.MouseDown[2] = mouseState[MouseButton.Middle];
            imIO.MousePos = new System.Numerics.Vector2(mouseState.X, mouseState.Y);
            imIO.MouseWheel  = mouseState.ScrollDelta.Y;
            imIO.MouseWheelH = mouseState.ScrollDelta.X;

            ImGuiMouseCursor currentCursor = ImGui.GetMouseCursor();
            if (lastCursor != currentCursor)
            {
                switch (currentCursor)
                {
                    case ImGuiMouseCursor.Hand:
                        window.Cursor = MouseCursor.Hand;
                        break;

                    case ImGuiMouseCursor.None:
                        window.Cursor = MouseCursor.Empty;
                        break;

                    case ImGuiMouseCursor.ResizeNS:
                        window.Cursor = MouseCursor.VResize;
                        break;

                    case ImGuiMouseCursor.ResizeEW:
                        window.Cursor = MouseCursor.HResize;
                        break;

                    case ImGuiMouseCursor.TextInput:
                        window.Cursor = MouseCursor.IBeam;
                        break;

                    default:
                        window.Cursor = MouseCursor.Default;    //Also arrow on windows. idk about this one.
                        break;
                }

                lastCursor = currentCursor;
            }

            //Update ImGUI Keyboard
            KeyboardState keyboardState = window.KeyboardState;

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown)
                {
                    continue;
                }

                imIO.KeysDown[(int)key] = keyboardState.IsKeyDown(key);
            }

            imIO.KeyCtrl = keyboardState[Keys.LeftControl] | keyboardState[Keys.RightControl];
            imIO.KeyAlt = keyboardState[Keys.LeftAlt] | keyboardState[Keys.RightAlt];
            imIO.KeyShift = keyboardState[Keys.LeftShift] | keyboardState[Keys.RightShift];
            imIO.KeySuper = keyboardState[Keys.LeftSuper] | keyboardState[Keys.RightSuper];

            //Update Viewport
            camera = Matrix44.CreateOrthographic(0, 0, window.ClientSize.X, window.ClientSize.Y, -2048f, 2048f);

            windowWidth = window.ClientSize.X;
            windowHeight = window.ClientSize.Y;
        }

        public static void OnTextInput(uint unicodeKey)
        {
            imIO.AddInputCharacter(unicodeKey);
        }

        public static void BeginFrame()
        {
            ImGui.NewFrame();
        }

        public static void EndFrame()
        {
            ImGui.EndFrame();
            ImGui.Render();

            glState.GetState();
            glState.SetState();

            //Bind Shader
            shader.Bind();
            shader.SetMatrix44("camera", ref camera);
            shader.SetSampler2D("fontTexture", 0);

            //Do ImGUI draw
            ImDrawDataPtr drawData = ImGui.GetDrawData();

            if (drawData.CmdListsCount > 0)
            {
                GL.BindVertexArray(vao);

                for (int i = 0; i < drawData.CmdListsCount; i++)
                {
                    ImDrawListPtr cmdList = drawData.CmdListsRange[i];

                    //Resize Buffers (if needed)
                    int vertexBufferSize = cmdList.VtxBuffer.Size * 20;
                    int indexBufferSize = cmdList.IdxBuffer.Size * 2;

                    if (maxVertexCount < cmdList.VtxBuffer.Size)
                    {
                        Log.Write("ImGui", 0x80FF80, $"Resizing Vertex Buffer [old size = {maxVertexCount}, new size = {cmdList.VtxBuffer.Size}]");
                        GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, IntPtr.Zero, BufferUsageHint.StreamDraw);
                        maxVertexCount = cmdList.VtxBuffer.Size;
                    }

                    if(maxIndexCount < cmdList.IdxBuffer.Size)
                    {
                        Log.Write("ImGui", 0x80FF80, $"Resizing Index Buffer [old size = {maxIndexCount}, new size = {cmdList.IdxBuffer.Size}]");
                        GL.BufferData(BufferTarget.ElementArrayBuffer, indexBufferSize, IntPtr.Zero, BufferUsageHint.StreamDraw);
                        maxIndexCount = cmdList.IdxBuffer.Size;
                    }

                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertexBufferSize, cmdList.VtxBuffer.Data);
                    GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, indexBufferSize, cmdList.IdxBuffer.Data);

                    for (int j = 0; j < cmdList.CmdBuffer.Size; j++)
                    {
                        ImDrawCmdPtr command = cmdList.CmdBuffer[j];

                        if(command.UserCallback != IntPtr.Zero)
                        {

                        }
                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, (int)command.TextureId);

                            GL.Scissor(
                                (int)command.ClipRect.X, 
                                (int)(windowHeight - command.ClipRect.W), 
                                (int)(command.ClipRect.Z - command.ClipRect.X), 
                                (int)(command.ClipRect.W - command.ClipRect.Y)
                            );

                            if ((imIO.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                            {
                                GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)command.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(command.IdxOffset * sizeof(ushort)), unchecked((int)command.VtxOffset));
                            }
                            else
                            {
                                GL.DrawElements(BeginMode.Triangles, (int)command.ElemCount, DrawElementsType.UnsignedShort, (int)command.IdxOffset * sizeof(ushort));
                            }
                        }
                    }           
                }

                glState.RestoreState();

                return;
            }
        }
    }
}
