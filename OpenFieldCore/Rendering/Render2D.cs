using System;
using System.Runtime.CompilerServices;

using OpenTK.Graphics.OpenGL;

using OFC.Resource.Texture;
using OFC.Numerics;
using OFC.Utility;

namespace OFC.Rendering
{
    public unsafe static partial class Render2D
    {
        /// <summary>
        /// Definitions which control the current state of the renderer
        /// </summary>
        [Flags]
        enum Render2DState : ushort
        {
            DrawOn = 0x0001,
            None   = 0x0000
        }

        /// <summary>
        /// Stores the current value of some GL Caps which must be restored.
        /// </summary>
        struct Render2DGLState
        {
            bool CullFace;
            bool DepthTest;
            bool Blend;
            int  BlendDst;
            int  BlendSrc;

            int[] Viewport;

            public void Capture()
            {
                //Grab enable bits
                CullFace  = GL.GetBoolean(GetPName.CullFace);
                DepthTest = GL.GetBoolean(GetPName.DepthTest);
                Blend     = GL.GetBoolean(GetPName.Blend);

                //Grab blending settings
                BlendDst = GL.GetInteger(GetPName.BlendDst);
                BlendSrc = GL.GetInteger(GetPName.BlendSrc);

                //Grab viewport
                Viewport = new int[4];
                GL.GetInteger(GetPName.Viewport, Viewport);
            }

            public void Restore()
            {
                EnableOrDisable(EnableCap.CullFace, CullFace);
                EnableOrDisable(EnableCap.DepthTest, DepthTest);
                EnableOrDisable(EnableCap.Blend, Blend);

                GL.BlendFunc((BlendingFactor)BlendSrc, (BlendingFactor)BlendDst);

                GL.Viewport(Viewport[0], Viewport[1], Viewport[2], Viewport[3]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EnableOrDisable(EnableCap cap, bool value)
            {
                if(value)
                {
                    GL.Enable(cap);
                }
                else
                {
                    GL.Disable(cap);
                }
            }
        }

        /// <summary>
        /// Specifices what vertex components are currently used in a batch
        /// </summary>
        [Flags]
        enum BatchVertexComponents : ushort
        {
            Position = 0x0001,
            Texcoord = 0x0002,
            Colour   = 0x0004,

            None     = 0x0000
        }

        /// <summary>
        /// Stores batch specific data as a compound data type
        /// </summary>
        struct BatchBuffer
        {
            //Constants
            public const int maxVertices   = 10000;
            public const int maxIndices    = 30000;
            public const int maxVertexSize = 8;      //Max Vertex Size is 8 (2 for position, 2 for texcoord, 1 for colour, 3 for padding)

            //OpenGL Objects
            public int glVAO;
            public int glVBO;
            public int glIBO;

            //Local CPU Objects
            public float[] componentBuffer;
            public ushort[] indexBuffer;

            public BatchBuffer()
            {
                //Initialize CPU Vertex Buffer
                componentBuffer = new float[maxVertexSize * maxVertices];

                //Initialize CPU Index Buffer
                indexBuffer = new ushort[maxIndices];

                //Initialize GL Vertex Array
                glVAO = GL.GenVertexArray();

                //Initialize GL Vertex Buffer
                glVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, glVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (sizeof(float) * maxVertexSize) * maxVertices, (IntPtr)null, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                //Initialize GL Index Buffer
                glIBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, glIBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(ushort) * maxIndices, (IntPtr)null, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        // Render2D State
        static Render2DState rendererState;
        static Render2DGLState rendererGLState;

        static bool isInitialized = false;

        // Batch Buffer Data
        const int maxBatchBuffers = 3;      //Always set this to some Pow2-1 number so cycling works. (1, 3, 7, 15 etc)
        static BatchBuffer[] batchBuffer;
        static int batchBufferID;
        static int batchCount;
        static int batchVertexCount;
        static int batchIndexCount;

        // Batch Texture Data
        static TextureResource batchTexture;
         
        // Batch Camera Data    
        static Vector2i viewSize = new(1024, 768);
        static Matrix4f camera   = Matrix4f.CreateOrthographic(0f, 0f, viewSize.X, viewSize.Y, -4096f, 4096f);

        // Batch Shaders
        static Shader shdColour;
        static Shader shdColourTexture;

        // Properties
        public static int Batches => batchCount;

        // Batch State
        static BatchVertexComponents batchComponents = BatchVertexComponents.None;

        public static void Initialize()
        {
            //Initialization Guard
            if (isInitialized)
                return;
            isInitialized = true;

            //Set Initial Render2D State
            rendererGLState = new Render2DGLState();
            rendererState = Render2DState.None;

            //Create Batch Buffers
            batchBuffer = new BatchBuffer[maxBatchBuffers];
            for(int i = 0; i < batchBuffer.Length; i++)
                batchBuffer[i] = new BatchBuffer();

            batchBufferID = 0;

            //Initialize Batch Shaders
            string[] shaderUniforms, shaderSamplers;

            shaderUniforms = new string[]
            {
                "camera"
            };
            shdColour = new Shader("Resources\\Shader\\Colour2D.vss", "Resources\\Shader\\Colour2D.fss", shaderUniforms);

            shaderUniforms = new string[]
            {
                "camera"
            };
            shaderSamplers = new string[]
            {
                "sDiffuse"
            };
            shdColourTexture = new Shader("Resources\\Shader\\ColourTexture2D.vss", "Resources\\Shader\\ColourTexture2D.fss", shaderUniforms, shaderSamplers);
            return;
        }

        public static void Resize(int width, int height)
        {
            viewSize = new Vector2i(width, height);
            camera   = Matrix4f.CreateOrthographic(0f, 0f, viewSize.X, viewSize.Y, -4096f, 4096f);
        }

        public static void DrawBegin()
        {
            //Initialization Guard - We should only really call this function once or twice per frame, so a branch here is fine.
            if (!isInitialized)
                return;

            if ((rendererState & Render2DState.DrawOn) > 0)
            {
                Log.Warn("Tried to call Render2D:DrawBegin while already drawing!");
                return;
            }

            //Set DrawOn Flag
            rendererState |= Render2DState.DrawOn;
            batchCount = 0;

            //Begin a batch (default)
            BatchBegin(BatchVertexComponents.None);

            //Capture GL State and set our required state for 2D Drawing
            rendererGLState.Capture();

            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);    //We don't want depth test in 2D...            
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Viewport(0, 0, viewSize.X, viewSize.Y);
        }

        public static void DrawEnd()
        {
            //Initialization Guard - We should only really call this function once or twice per frame, so a branch here is fine.
            if (!isInitialized)
                return;

            if ((rendererState & Render2DState.DrawOn) == 0)
            {
                Log.Warn("Tried to call Render2D:DrawEnd while not drawing!");
                return;
            }

            //Unset DrawOn Flag
            rendererState ^= Render2DState.DrawOn;

            //End current batch
            BatchEnd();

            //Restore GL State
            rendererGLState.Restore();
        }
    }
}
