using OFC.Resource.Format;
using OFC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OFC.Resource.Model;

namespace OFC.Resource.FileFormat
{
    public partial class MS3DFormat : IFormat<ModelResource>
    {
        public SFormatParameters Parameters => parameters;

        public bool Load(string filepath, ref ModelResource asset, in object parameters)
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

        public bool Load(byte[] buffer, ref ModelResource asset, in object parameters)
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

        public bool Save(string filepath, ModelResource asset)
        {
            throw new NotImplementedException();
        }
    }
}
