using System.Text.Json;
using System.IO;

using OFC.Utility;
using OFC.Numerics;

namespace OFC.Rendering
{
    public class Font
    {
        struct Atlas
        {
            public string name { get; set; }
            public string file { get; set; }
        }

        public struct JSONGlyph
        {
            public int unicode { get; set; }
            public float advance { get; set; }
            public float planeLeft { get; set; }
            public float planeBottom { get; set; }
            public float planeRight { get; set; }
            public float planeTop { get; set; }
            public float atlasLeft { get; set; }
            public float atlasBottom { get; set; }
            public float atlasRight { get; set; }
            public float atlasTop { get; set; }
        }

        struct JSONFontFile
        {
            public Atlas atlas { get; set; }
            public JSONGlyph[] glyphs { get; set; }
        }

        public struct Glyph
        {
            public Vector2f uv;
            public Vector2f uv2;
        }

        //Data
        //public Texture fontAtlas;
        public JSONGlyph[] glyphs;

        public Font(string fontPath)
        {
            //Load Font Glyphs etc
            string everyJsonLine = File.ReadAllText(fontPath);
            JSONFontFile file = JsonSerializer.Deserialize<JSONFontFile>(everyJsonLine);
            Log.Info($"New Font [name = {file.atlas.name}, atlas = {file.atlas.file}]");

            //Load Font Atlas
            /*
            TextureFactory textureFactory = new TextureFactory();
            if (!textureFactory.GetHandler(0).Load(Path.Combine(Path.GetDirectoryName(fontPath), file.atlas.file), out TextureAsset asset))
            {
                Log.Error("Failed to load font atlas!");
                return;
            }
            fontAtlas = new Texture(asset);
            glyphs = file.glyphs;
            */
        }
    }
}
