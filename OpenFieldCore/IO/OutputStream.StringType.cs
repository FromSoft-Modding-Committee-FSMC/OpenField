using System.Text;

namespace OFC.IO
{
    public partial class OutputStream
    {
        public void WriteString(string v)
        {
            byte[] vbytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(v));
            Write7BitEncodedInt(Encoding.UTF8.GetCharCount(vbytes));
            fstream.Write(vbytes, 0, vbytes.Length);
        }

        public void WriteString(string v, Encoding encoding)
        {
            byte[] vbytes = Encoding.Convert(Encoding.Default, encoding, Encoding.Default.GetBytes(v));
            Write7BitEncodedInt(encoding.GetCharCount(vbytes));
            fstream.Write(vbytes, 0, vbytes.Length);
        }

        public void WriteRawString(string v)
        {
            byte[] vbytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(v));
            fstream.Write(vbytes, 0, vbytes.Length);
        }
    }
}
