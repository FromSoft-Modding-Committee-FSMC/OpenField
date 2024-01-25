using System.ComponentModel;

namespace OFC.Resource.Texture
{
    /// <summary>
    /// Configuration options for texture loading
    /// </summary>
    public struct STextureLoadParameters
    {
        /// <summary>
        /// Default configuration for loading textures.
        /// </summary>
        public static readonly STextureLoadParameters Default = new()
        {
            mipmapsGenerate = true,
            mipmapsLoad     = false,
            mipmapsMethod   = ETextureMipgenMethod.Box2x2,
            mipmapsMax      = 4
        };

        /// <summary>
        /// Set to generate mipmaps.
        /// </summary>
        public bool mipmapsGenerate;

        /// <summary>
        /// Set to load mipmaps from the file (if they exist).
        /// This setting should be ignored if 'mipmapsGenerate' is set,
        /// but a warning should be issued.
        /// </summary>
        public bool mipmapsLoad;

        /// <summary>
        /// The method used for generating mipmaps.
        /// </summary>
        public ETextureMipgenMethod mipmapsMethod;

        /// <summary>
        /// The maximum amount of mipmaps that are loaded or generated.
        /// </summary>
        public int mipmapsMax;
    }
}
