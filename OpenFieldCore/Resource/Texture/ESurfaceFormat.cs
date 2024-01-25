namespace OFC.Resource.Texture
{
    /// <summary>
    /// ESurfaceFormat declares the colour mode of a surface.
    /// </summary>
    public enum ESurfaceFormat : uint
    {
        /// <summary>
        /// Direct RGB Format with 8 bits for each of R, G and B (U8)
        /// </summary>
        DR8G8B8ui,

        /// <summary>
        /// Direct RGB Format with 8 bits for each of B, G and R (U8)
        /// </summary>
        DB8G8R8ui,

        /// <summary>
        /// Direct RGBA Format with 8 bits for each of R, G, B and A (U8)
        /// </summary>
        DR8G8B8A8ui,

        /// <summary>
        /// Direct RGBA Format with 8 bits for each of B, G, R and A (U8)
        /// </summary>
        DB8G8R8A8ui,

        /// <summary>
        /// Indexed X Format with 4 bits per palette index
        /// </summary>
        I4,

        /// <summary>
        /// Indexed X Format with 8 bits per palette index
        /// </summary>
        I8
    }
}
