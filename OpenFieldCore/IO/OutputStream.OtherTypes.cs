namespace OFC.IO
{
    public partial class OutputStream
    {
        public void WriteBCD8(byte v)
        {
            fstream.WriteByte((byte)(((v / 10) << 4) | v % 10));
        }

        public void WriteBytes(byte[] v)
        {
            fstream.Write(v, 0, v.Length);
        }

        public unsafe void Write7BitEncodedInt(int value)
        {
            uint vu = *(uint*)&value;
            while (vu >= 0x80)
            {
                fstream.WriteByte((byte)((vu & 0x7F) | 0x80));
                vu >>= 7;
            }
            fstream.WriteByte((byte)vu);
        }
    }
}