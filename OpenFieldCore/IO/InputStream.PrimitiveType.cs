using System;
using System.Buffers.Binary;

namespace OFC.IO
{
    public partial class InputStream
    {
        //Signed Primitives
        public sbyte ReadS8()
        {
            fstream.ReadExactly(buffer, 0, 1);
            return (sbyte)buffer[0];
        }

        public short ReadS16(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 2);

            short v = (short)(buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        public int ReadS32(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 4);

            int v = (buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        public long ReadS64(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 8);

            long v = (buffer[7] << 56 | buffer[6] << 48 | buffer[5] << 40 | buffer[4] << 32 | buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        //Unsigned Primitives
        public byte ReadU8()
        {
            fstream.ReadExactly(buffer, 0, 1);
            return buffer[0];
        }

        public ushort ReadU16(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 2);

            ushort v = (ushort)(buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        public uint ReadU32(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 4);

            uint v = (uint)(buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        public ulong ReadU64(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 8);

            ulong v = (ulong)(buffer[7] << 56 | buffer[6] << 48 | buffer[5] << 40 | buffer[4] << 32 | buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
                v = BinaryPrimitives.ReverseEndianness(v);

            return v;
        }

        //Fractional Primitives
        public Half ReadF16(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 2);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
            {
                //Shuffle buffered bytes to swap endianness
                buffer[2] = buffer[0];
                buffer[0] = buffer[1];
                buffer[1] = buffer[2];
            }

            return BitConverter.ToHalf(buffer, 0);
        }

        public float ReadF32(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 4);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
            {
                //Shuffle buffered bytes to swap endianness
                buffer[4] = buffer[0];  //Swap Bytes 0 & 3
                buffer[0] = buffer[3];
                buffer[3] = buffer[4];
                buffer[4] = buffer[1];  //Swap Bytes 1 & 2
                buffer[1] = buffer[2];
                buffer[2] = buffer[4];
            }

            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadF64(EEndianness endianness = EEndianness.Little)
        {
            fstream.ReadExactly(buffer, 0, 8);

            if ((endianness == EEndianness.Big && BitConverter.IsLittleEndian) || (endianness == EEndianness.Little && !BitConverter.IsLittleEndian))
            {
                //Shuffle buffered bytes to swap endianness
                buffer[8] = buffer[0];  //Swap bytes 0 & 7
                buffer[0] = buffer[7];
                buffer[7] = buffer[8];
                buffer[8] = buffer[1];  //Swap bytes 1 & 6
                buffer[1] = buffer[6];
                buffer[6] = buffer[8];
                buffer[8] = buffer[2];  //Swap bytes 2 & 5
                buffer[2] = buffer[5];
                buffer[5] = buffer[8];
                buffer[8] = buffer[3];  //Swap bytes 3 & 4
                buffer[3] = buffer[4];
                buffer[4] = buffer[8];
            }

            return BitConverter.ToDouble(buffer, 0);
        }
    }
}
