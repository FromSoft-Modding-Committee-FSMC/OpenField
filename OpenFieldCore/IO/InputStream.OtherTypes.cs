using System.Text;

namespace OFC.IO
{
    public partial class InputStream
    {
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            fstream.ReadExactly(bytes, 0, count);

            return bytes;
        }

        public unsafe ushort[] ReadU16s(int count)
        {
            byte[] bytes = new byte[count * 2];
            fstream.ReadExactly(bytes, 0, count * 2);

            ushort[] u16s = new ushort[count];
            for (int i = 0; i < count; ++i)
                u16s[i] = (ushort)(bytes[(2 * i) + 0] << 0 | bytes[(2 * i) + 1] << 8);

            return u16s;
        }

        public char[] ReadFixedString(int length)
        {
            byte[] bytes = new byte[length];
            fstream.ReadExactly(bytes, 0, length);

            return Encoding.UTF8.GetString(bytes, 0, length).ToCharArray();
        }
    }
}
