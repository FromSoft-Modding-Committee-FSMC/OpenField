using OpenTK.Graphics.OpenGL;

using OFC.Resource.Texture;
using OFC.Utility;
using System;
using OpenTK.Compute.OpenCL;
using OFC.Rendering;

namespace OFC.Resource.Texture
{
    public partial class TextureResource : IResource
    {
        // CPU Data - IResource
        public EResourceState State { get; set; }
        public EResourceFlags Flags { get; set; }
        public int ReferenceCount { get; set; }
        public string Source { get; set; }
        public uint Hash { get; set; }

        // CPU Data
        ETextureMode textureMode;
        Surface[] surfaces;
        int surfaceCount;

        // CPU Properties
        public ETextureMode TextureMode => textureMode;
        public int SurfaceCount => surfaceCount;
        public int SurfaceMax   => surfaces.Length;

        // GPU Data
        Surface gpuTransferSurface = null;
        int gpuTransferStage;
        int gpuTexture;

        /// <summary>
        /// Constructs a new TextureResource
        /// </summary>
        public TextureResource() 
        { }


        /// <summary>
        /// Constructs a new TextureResource.
        /// </summary>
        /// <param name="mode">The type of data the texture will contain</param>
        /// <param name="maxSurfaceCount">The maximum amount of surfaces the texture can contain</param>
        public TextureResource(ETextureMode mode, int maxSurfaceCount)
        {
            Reserve(maxSurfaceCount);
            SetMode(mode);
        }


        /// <summary>
        /// Reserves a number of slots for surface storage.
        /// </summary>
        /// <param name="maxSurfaceCount">The number of slots to reserve.</param>
        public void Reserve(int maxSurfaceCount)
        {
            surfaces = new Surface[maxSurfaceCount];
            surfaceCount = 0;
        }


        /// <summary>
        /// Sets the mode of the texture resource.
        /// </summary>
        /// <param name="mode">The texture resource mode</param>
        public void SetMode(ETextureMode mode)
        {
            textureMode = mode;
        }


        /// <summary>
        /// Loads a surface into the texture resource.
        /// </summary>
        /// <param name="surface">The surface to load</param>
        /// <returns>Internal ID of the loaded surface</returns>
        public int LoadSurface(Surface surface)
        {
            surfaces[surfaceCount] = surface;
            return surfaceCount++;
        }


        /// <summary>
        /// Binds texture resource for rendering
        /// </summary>
        /// <param name="samplerUnit">The sampler unit to bind to</param>
        public void Bind(int samplerUnit = 0)
        {
            // Branch for GPUTransfer
            if (State != EResourceState.Ready)
            {
                // Check for GL Errors during transfer
                ErrorCode glError;

                switch (State)
                {
                    case EResourceState.WaitGPUTransfer:
                        TransferBegin();
                        glError = GL.GetError();
                        if (glError != ErrorCode.NoError)
                            Log.Error($"GL Error during TextureResource WaitGPUTransfer -> Begin [error = {glError}]!");

                        TransferContinue();
                        glError = GL.GetError();
                        if (glError != ErrorCode.NoError)
                            Log.Error($"GL Error during TextureResource WaitGPUTransfer -> Continue [error = {glError}]!");
                        break;

                    case EResourceState.InGPUTransfer:
                        TransferContinue();

                        glError = GL.GetError();
                        if (glError != ErrorCode.NoError)
                            Log.Error($"GL Error during TextureResource GPUTransferContinue [error = {glError}]!");
                        break;

                    // Any other state at this point is an error.
                    default:
                        Log.Error($"Invalid TextureResource state during GPUTransfer [state = {State}]!");
                        return;
                }
            }

            // Bind whatever we have at the moment
            GL.BindTextureUnit(samplerUnit, gpuTexture);

            // Place Hash into RenderContext
            RenderContext.CurrentTexture = Hash;
        }


        /// <summary>
        /// Begins transfer of immediate data to the GPU
        /// </summary>
        private void TransferBegin()
        {
            // Get entry point surface
            gpuTransferSurface = surfaces[0];

            // Generate initial GPU Resource
            switch(textureMode)
            {
                // Configure as Texture2D type
                case ETextureMode.Texture2D:
                    GL.CreateTextures(TextureTarget.Texture2D, 1, out gpuTexture);
                    GL.TextureStorage2D(gpuTexture, 1 + gpuTransferSurface.MipmapCount, GetGLSizedFormat(gpuTransferSurface.Format), gpuTransferSurface.Width, gpuTransferSurface.Height);
                    GL.TextureParameter(gpuTexture, TextureParameterName.TextureMaxLevel, gpuTransferSurface.MipmapCount);
                    GL.TextureParameter(gpuTexture, TextureParameterName.TextureBaseLevel, gpuTransferSurface.MipmapCount);

                    break;

                case ETextureMode.Texture2DArray:
                case ETextureMode.Texture3D:
                case ETextureMode.TextureCube:
                    Log.Error($"Unsupported Texture Mode! [mode = {textureMode}, source = {Source}]");
                    State = EResourceState.Failed;
                    return;

                default:
                    Log.Error($"Unknown Texture Mode! [source = {Source}]");
                    State = EResourceState.Failed;
                    return;
            }

            State = EResourceState.InGPUTransfer;
        }


        /// <summary>
        /// Continues transfer of immediate data to the GPU
        /// </summary>
        private void TransferContinue()
        {
            // Depending on texture mode, we must transfer data differently.
            switch(textureMode)
            {
                case ETextureMode.Texture2D:
                    // Transfer Mipmap
                    if (gpuTransferStage < gpuTransferSurface.MipmapCount)
                    {
                        // Get surface for mipmap from primary surface
                        int mipLevel    = (gpuTransferSurface.MipmapCount - gpuTransferStage);
                        int mipID       = gpuTransferSurface.MipmapIDs[mipLevel - 1];
                        Surface mipSurf = surfaces[mipID];                      

                        // Load Those Pixels               
                        GL.TextureSubImage2D(gpuTexture, mipLevel, 0, 0, mipSurf.Width, mipSurf.Height, GetGLPixelFormat(mipSurf.Format), GetGLPixelType(mipSurf.Format), mipSurf.Buffer);
                        GL.TextureParameter(gpuTexture, TextureParameterName.TextureBaseLevel, mipLevel);

                        gpuTransferStage++;
                        return;
                    }

                    // Transfer Main Surface
                    GL.TextureSubImage2D(gpuTexture, 0, 0, 0, gpuTransferSurface.Width, gpuTransferSurface.Height, GetGLPixelFormat(gpuTransferSurface.Format), GetGLPixelType(gpuTransferSurface.Format), gpuTransferSurface.Buffer);
                    GL.TextureParameter(gpuTexture, TextureParameterName.TextureBaseLevel, 0);

                    // Prepare Next Surface
                    gpuTransferSurface = gpuTransferSurface.Next == -1 ? null : surfaces[gpuTransferSurface.Next];
                    
                    break;

                case ETextureMode.Texture2DArray:
                case ETextureMode.Texture3D:
                case ETextureMode.TextureCube:
                    Log.Error($"Unsupported Texture Mode! [mode = {textureMode}, source = {Source}]");
                    State = EResourceState.Failed;
                    return;

                default:
                    Log.Error($"Unknown Texture Mode! [source = {Source}]");
                    State = EResourceState.Failed;
                    return;
            }

            // If we are still transfering data, this check will fail.
            if (gpuTransferSurface != null)
                return;

            State = EResourceState.Ready;
        }
    }
}
