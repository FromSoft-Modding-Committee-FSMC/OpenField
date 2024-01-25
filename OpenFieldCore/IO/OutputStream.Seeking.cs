using System.IO;

namespace OFC.IO
{
    public partial class OutputStream
    {
        /// <summary>
        /// Seeks to an offset relative to the beginning of the stream
        /// </summary>
        /// <param name="offset">Position from start</param>
        /// <returns>The old position</returns>
        public long SeekBegin(long offset)
        {
            long oldPosition = Position;
            fstream.Seek(offset, SeekOrigin.Begin);
            return oldPosition;
        }

        /// <summary>
        /// Seeks to an offset relative to the end of the stream
        /// </summary>
        /// <param name="offset">Position from end</param>
        /// <returns>The old position</returns>
        public long SeekEnd(long offset)
        {
            long oldPosition = Position;
            fstream.Seek(offset, SeekOrigin.End);
            return oldPosition;
        }

        /// <summary>
        /// Seeks to an offset relative to the current position of the stream
        /// </summary>
        /// <param name="offset">Position from the current position</param>
        /// <returns>The old position</returns>
        public long SeekRelative(long offset)
        {
            long oldPosition = Position;
            fstream.Seek(offset, SeekOrigin.Current);
            return oldPosition;
        }

        /// <summary>
        /// Seeks to an offset relative to the beginning of the stream, but adds the old position
        /// to a stack so it can be restored.
        /// </summary>
        /// <param name="offset">Position from start</param>
        /// <returns>The old position</returns>
        public long Jump(long offset)
        {
            jumpStack.Push(Position);
            fstream.Seek(offset, SeekOrigin.Begin);
            return jumpStack.Peek();
        }

        /// <summary>
        /// Returns to the last offset that was jumped from
        /// </summary>
        /// <returns>the old position</returns>
        public long Return()
        {
            long oldPosition = Position;
            fstream.Seek(jumpStack.Pop(), SeekOrigin.Begin);
            return oldPosition;
        }
    }
}
