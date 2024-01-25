using OFC.Resource.Format;
using OFC.IO;
using OFC.Resource.Texture;
using System;

namespace OFC.Resource.FileFormat
{
    public partial class TGAFormat : IFormat<TextureResource>
    {
        public SFormatParameters Parameters => parameters;

        public bool Load(string filepath, ref TextureResource asset, in object parameters)
        {
            try
            {
                using InputStream ins = new(filepath);
                LoadFromStream(ins, ref asset, in parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                asset = null;
                return false;
            }

            return true;
        }

        public bool Load(byte[] buffer, ref TextureResource asset, in object parameters)
        {
            try
            {
                using InputStream ins = new(buffer);
                LoadFromStream(ins, ref asset, in parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                asset = null;
                return false;
            }

            return true;
        }

        public bool Save(string filepath, TextureResource asset)
        {
            throw new NotImplementedException();
        }
    }
}
