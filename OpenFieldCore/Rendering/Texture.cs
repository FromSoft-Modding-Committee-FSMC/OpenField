using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;

using OFC.Asset;
using OFC.Utility;
using OFC.IO.Hashing;
using MoonSharp.Interpreter;
using System.Reflection.Emit;

namespace OFC.Rendering
{
    public class Texture
    {
        private readonly uint _hash;

        private int glTexture;
        private TextureTarget glTextureTarget = TextureTarget.Texture2D;

        public Texture(TextureAsset asset)
        {
            TextureSubimage subimage;
            ColourMode colourMode = ColourMode.Unknown;
            int maxWidth = 0, maxHeight = 0;

            //Varify subimages
            for(int i = 0; i < asset.SubImageCount; ++i)
            {
                asset.GetSubImage(i, out subimage);

                if (subimage.mode == ColourMode.Unknown)
                {
                    Log.Warn($"TextureAsset contains invalid subimage");
                    return;
                }

                if (colourMode == ColourMode.Unknown)
                {
                    colourMode = subimage.mode;
                }
                else
                {
                    if (colourMode != subimage.mode)
                    {
                        Log.Warn($"TextureAsset contains subimages of different modes! [mode: {colourMode}]");
                        return;
                    }
                }

                if (subimage.width > maxWidth)
                {
                    maxWidth = (int)subimage.width;
                }

                if (subimage.height > maxHeight)
                {
                    maxHeight = (int)subimage.height;
                }
            }

            if(colourMode == ColourMode.Unknown || maxWidth == 0 || maxHeight == 0)
            {
                Log.Warn($"TextureAsset does not contain any valid subimages.");
                return;
            }

            //Create a texture array from each subimage
            glTexture = GL.GenTexture();
            GLError("GenTexture");
            GL.BindTexture(TextureTarget.Texture2DArray, glTexture);
            GLError("BindTexture");
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba8, maxWidth, maxHeight, asset.SubImageCount, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GLError("TexStorage3D");

            IHashFunction fnvHasher = new FNV32();

            for(int i = 0; i < asset.SubImageCount; ++i)
            {
                asset.GetSubImage(i, out subimage);

                if (_hash == 0)  //Only take the hash of the first texture.
                {
                    _hash = BitConverter.ToUInt32(fnvHasher.GetHash(ref subimage.buffer));
                    Log.Info($"New Texture [FNV32: {_hash:X8}]");
                }

                switch(subimage.mode)
                {
                    case ColourMode.M4:
                        Log.Error("Cannot generate GL Texture from asset! M4 mode textures are not supported.");
                        return;

                    case ColourMode.M8:
                        Log.Error("Cannot generate GL Texture from asset! M8 mode textures are not supported.");
                        return;

                    case ColourMode.D16:
                        Log.Error("Cannot generate GL Texture from asset! D16 mode textures are not supported.");
                        return;

                    case ColourMode.D24:
                        LoadD24Texture(this, ref subimage, i);
                        break;

                    case ColourMode.D32:
                        LoadD32Texture(this, ref subimage, i);
                        break;

                    case ColourMode.F128:
                        LoadF128Texture(this, ref subimage, i);
                        break;
                }
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GLError("GenerateMipmap");
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
            GLError("BindTexture 0");
        }

        public void Bind(int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2DArray, glTexture);
        }

        //Image Load Helpers (by mode)
        private static bool LoadD24Texture(Texture texture, ref TextureSubimage subimage, int layer)
        {
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, (int)subimage.width, (int)subimage.height, 1, PixelFormat.Rgb, PixelType.UnsignedByte, subimage.buffer);
            GLError("D24 TexSubimage3D");
            return true;
        }
        private static bool LoadD32Texture(Texture texture, ref TextureSubimage subimage, int layer)
        {
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, (int)subimage.width, (int)subimage.height, 1, PixelFormat.Rgba, PixelType.UnsignedByte, subimage.buffer);
            GLError("D32 TexSubimage3D");
            return true;
        }
        private static bool LoadF128Texture(Texture texture, ref TextureSubimage subimage, int layer)
        {
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, (int)subimage.width, (int)subimage.height, 1, PixelFormat.Rgba, PixelType.Float, subimage.buffer);
            GLError("F128 TexSubimage3D");
            return true;
        }

        private static PixelInternalFormat ColourModeToPIF(ColourMode mode)
        {
            switch(mode)
            {
                case ColourMode.M4:
                case ColourMode.M8:
                case ColourMode.D32:
                case ColourMode.D24:
                case ColourMode.D16:
                    return PixelInternalFormat.Rgba8;

                case ColourMode.F128:
                    return PixelInternalFormat.Rgba32f;
            }

            return PixelInternalFormat.Rgba8;
        }

        private static void GLError(string name)
        {
            ErrorCode err = GL.GetError();
            if(err != ErrorCode.NoError)
            {
                Log.Error($"GL Error ({name})! [code: {err}]");
            }
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Texture)
            {
                return obj.GetHashCode() == GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)_hash;
        }
    }
}
