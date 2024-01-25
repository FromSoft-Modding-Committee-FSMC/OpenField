using OFC.Resource.Format;
using OFC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.FileFormat
{
    public partial class MS3DFormat
    {
        SFormatParameters parameters = new()
        {
            metadata = new SFormatMetadata()
            {
                name = "MilkShape 3D",
                description = "3D Model Format",
                version = "4",
                specRevisionDate = "Approx. 2014/8/1",
                authors = new string[] { "Mete Ciragan" },
                categories = new string[] { "Binary", "Skinned" },
                extensions = new string[] { "ms3d" }
            },

            allowImport = true,
            allowExport = false,

            validator = ValidateMS3DImport
        };

        static bool ValidateMS3DImport(byte[] buffer)
        {
            bool isValid = true;

            using (InputStream ins = new InputStream(buffer))
            {
                //Quickly validate the header. Very easy with MS3D.
                isValid &= (ins.ReadU32() == 0x4433534D);   //Tag == 'MS3D000000'
                isValid &= (ins.ReadU32() == 0x30303030);   // ^ ^ ^ ^ ^ ^ ^ ^ ^
                isValid &= (ins.ReadU16() == 0x3030);       // ^ ^ ^ ^ ^ ^ ^ ^ ^

                isValid &= (ins.ReadU32() == 4);            //Version ==  4
            }

            return isValid;
        }
    }
}
