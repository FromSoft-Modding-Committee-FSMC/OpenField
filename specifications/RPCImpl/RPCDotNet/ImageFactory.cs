using Mixolydian.IO.Asset;
using Mixolydian.IO.Format;
using Mixolydian.IO.Format.Sony;
using Mixolydian.IO.Format.ForFolks;

namespace RPCTool
{
    public class ImageFactory : IFormatFactory<ImageAsset>
    {
        public ImageFactory()
        {
            RegisterFormat(new TIMFormat());
            RegisterFormat(new RPCFormat());
        }
    }
}
