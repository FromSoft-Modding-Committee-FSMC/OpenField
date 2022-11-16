using System.Collections.Generic;
using System.IO;

namespace OFC.IO
{
    public class BinaryInputStream : BinaryReader
    {
        //Data
        private readonly Stack<long> jumpStack;
        private uint bitBuffer;
        private uint bitBufferPosition;

        //Properties
        public long Length
        {
            get
            {
                return BaseStream.Length;
            }
        }
        public long Position
        {
            get
            {
                return this.BaseStream.Position;
            }
        }
        public bool EndOfStream
        {
            get { return BaseStream.Position == BaseStream.Length; }
        }

        public BinaryInputStream(string filepath) : base(File.OpenRead(filepath))
        {
            jumpStack = new Stack<long>();
        }
        public BinaryInputStream(byte[] buffer) : base(new MemoryStream(buffer))
        {
            jumpStack = new Stack<long>();
        }

        /// <summary>
        /// Stores the current stream position on a stack, and then jumps to a new position.
        /// </summary>
        /// <param name="offset">The position to jump too</param>
        public void Jump(long offset)
        {
            jumpStack.Push(BaseStream.Position);
            BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns to the last position stored in the stack.
        /// </summary>
        public void Return()
        {
            BaseStream.Seek(jumpStack.Pop(), System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// Seek to a new position in the stream.
        /// </summary>
        /// <param name="offset">The seek offset.</param>
        /// <param name="origin">The seek origin.</param>
        public void Seek(long offset, SeekOrigin origin)
        {
            BaseStream.Seek(offset, (System.IO.SeekOrigin) origin);
        }

        /// <summary>
        /// Reads a number of bits from the InputStream. A hard max of 32 bits is imposed.
        /// </summary>
        /// <param name="count">The number of bits.</param>
        /// <returns></returns>
        public uint ReadUnsignedBits(int count)
        {
            uint bitAccumulator = 0;

            for(int i = 0; i < count; ++i)
            {
                if((bitBufferPosition % 7) == 0)
                {
                    bitBuffer = ReadByte();
                    bitBufferPosition = 0;
                }

                bitAccumulator = (bitAccumulator << 1) | (bitBuffer & 0x1);
                bitBuffer >>= 1;

                bitBufferPosition++;
            }

            return bitAccumulator;
        }

        /// <summary>
        /// Reads a fixed string from the input stream.
        /// </summary>
        /// <param name="length">Fixed length of the string</param>
        /// <returns>The fixed string</returns>
        public string ReadString(int length)
        {
            return System.Text.Encoding.UTF8.GetString(ReadBytes(length));   
        }
    }
}