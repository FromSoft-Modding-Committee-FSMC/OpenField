using OFC.Resource.Format;
using OFC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Resource.FileFormat
{
    public partial class TGAFormat
    {
        SFormatParameters parameters = new()
        {
            metadata = new SFormatMetadata()
            {
                name = "[T]ruevision [G]raphics [A]dapter",
                description = "Raster image format designed for TARGA and VISTA graphics cards.",
                version = "2",
                specRevisionDate = "Approx. 1989",
                authors = new string[] { "Truevision, Inc." },
                categories = new string[] { "Proprietary", "Binary" },
                extensions = new string[] { "tga", "icb", "vda", "vst" }
            },

            allowImport = true,
            allowExport = false,
            
            validator = ValidateTGAImport
        };

        static bool ValidateTGAImport(byte[] buffer)
        {
            bool isValid = true;
            using (InputStream ins = new InputStream(buffer))
            {
                //Skip the first TGA field
                ins.SeekRelative(1);
                isValid &= (ins.ReadU8() < 2);  //Palette Type
                isValid &= (ins.ReadU8() < 3);  //Surface Type

                //Now we go to the end of the file -18 bytes, and validate part of the file footer.
                ins.SeekEnd(-18);
                isValid &= (new string(ins.ReadFixedString(16)) == "TRUEVISION-XFILE");
                isValid &= (ins.ReadU8() == 0x2E);
                isValid &= (ins.ReadU8() == 0x00);
            }
            return isValid;
        }
    }
}
