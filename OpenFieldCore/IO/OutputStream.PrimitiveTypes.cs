using System;
using System.Buffers.Binary;

namespace OFC.IO
{
    public partial class OutputStream
    {
        //Signed Primitives
        public void WriteS8(sbyte v)
        {
            fstream.WriteByte((byte)v);
        }

        public void WriteS16(short v, EEndianness endianness = EEndianness.Little)
        {
            if((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 0) & 0xFF);
            buffer[1] = (byte)((v >> 8) & 0xFF);
            
            fstream.Write(buffer, 0, 2);
        }

        public void WriteS32(int v, EEndianness endianness = EEndianness.Little)
        {
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 00) & 0xFF);
            buffer[1] = (byte)((v >> 08) & 0xFF);
            buffer[2] = (byte)((v >> 16) & 0xFF);
            buffer[3] = (byte)((v >> 24) & 0xFF);

            fstream.Write(buffer, 0, 4);
        }

        public void WriteS64(long v, EEndianness endianness = EEndianness.Little)
        {
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 00) & 0xFF);
            buffer[1] = (byte)((v >> 08) & 0xFF);
            buffer[2] = (byte)((v >> 16) & 0xFF);
            buffer[3] = (byte)((v >> 24) & 0xFF);
            buffer[4] = (byte)((v >> 32) & 0xFF);
            buffer[5] = (byte)((v >> 40) & 0xFF);
            buffer[6] = (byte)((v >> 48) & 0xFF);
            buffer[7] = (byte)((v >> 56) & 0xFF);

            fstream.Write(buffer, 0, 8);
        }

        //Unsigned Primitives
        public void WriteU8(byte v)
        {
            fstream.WriteByte(v);
        }

        public void WriteU16(ushort v, EEndianness endianness = EEndianness.Little)
        {
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 0) & 0xFF);
            buffer[1] = (byte)((v >> 8) & 0xFF);

            fstream.Write(buffer, 0, 2);
        }
     
        public void WriteU32(uint v, EEndianness endianness = EEndianness.Little)
        {
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 00) & 0xFF);
            buffer[1] = (byte)((v >> 08) & 0xFF);
            buffer[2] = (byte)((v >> 16) & 0xFF);
            buffer[3] = (byte)((v >> 24) & 0xFF);

            fstream.Write(buffer, 0, 4);
        }

        public void WriteU64(ulong v, EEndianness endianness = EEndianness.Little)
        {
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            buffer[0] = (byte)((v >> 00) & 0xFF);
            buffer[1] = (byte)((v >> 08) & 0xFF);
            buffer[2] = (byte)((v >> 16) & 0xFF);
            buffer[3] = (byte)((v >> 24) & 0xFF);
            buffer[4] = (byte)((v >> 32) & 0xFF);
            buffer[5] = (byte)((v >> 40) & 0xFF);
            buffer[6] = (byte)((v >> 48) & 0xFF);
            buffer[7] = (byte)((v >> 56) & 0xFF);

            fstream.Write(buffer, 0, 8);
        }

        //Fractional Primitives
        public void WriteF16(Half v, EEndianness endianness = EEndianness.Little)
        {  
            ushort vi = BitConverter.HalfToUInt16Bits(v);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                vi = BinaryPrimitives.ReverseEndianness(vi);

            buffer[0] = (byte)((vi >> 0) & 0xFF);
            buffer[1] = (byte)((vi >> 8) & 0xFF);

            fstream.Write(buffer, 0, 2);
        }

        public void WriteF32(float v, EEndianness endianness = EEndianness.Little)
        {
            uint vi = BitConverter.SingleToUInt32Bits(v);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                vi = BinaryPrimitives.ReverseEndianness(vi);

            buffer[0] = (byte)((vi >> 00) & 0xFF);
            buffer[1] = (byte)((vi >> 08) & 0xFF);
            buffer[2] = (byte)((vi >> 16) & 0xFF);
            buffer[3] = (byte)((vi >> 24) & 0xFF);

            fstream.Write(buffer, 0, 4);
        }
       
        public void WriteF64(double v, EEndianness endianness = EEndianness.Little)
        {
            ulong vi = BitConverter.DoubleToUInt64Bits(v);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                vi = BinaryPrimitives.ReverseEndianness(vi);

            buffer[0] = (byte)((vi >> 00) & 0xFF);
            buffer[1] = (byte)((vi >> 08) & 0xFF);
            buffer[2] = (byte)((vi >> 16) & 0xFF);
            buffer[3] = (byte)((vi >> 24) & 0xFF);
            buffer[4] = (byte)((vi >> 32) & 0xFF);
            buffer[5] = (byte)((vi >> 40) & 0xFF);
            buffer[6] = (byte)((vi >> 48) & 0xFF);
            buffer[7] = (byte)((vi >> 56) & 0xFF);

            fstream.Write(buffer, 0, 8);
        }
    }
}
