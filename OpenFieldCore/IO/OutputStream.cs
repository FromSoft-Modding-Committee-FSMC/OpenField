using System;
using System.IO;
using System.Collections.Generic;

namespace OFC.IO
{
    public partial class OutputStream : IDisposable
    {
        //Private Constants
        private const int defaultBufferSize = 16;
        private const int defaultTextBufferSize = 2048;

        //Private Data
        private readonly Stack<long> jumpStack;
        private Stream fstream;
        private readonly byte[] buffer;
        private readonly byte[] textBuffer;
        private int textPos;

        //Public Properties
        /// <summary>
        /// The current byte position of the stream.
        /// </summary>
        public long Position => fstream.Position;

        /// <summary>
        /// The current byte size of the stream.
        /// </summary>
        public long Size => fstream.Length;


        //Constructors
        /// <summary>
        /// Constructs a new OutputStream from a file.
        /// </summary>
        /// <param name="filepath">Path to the file</param>
        public OutputStream(string filepath) : this(File.Create(filepath)) { }

        /// <summary>
        /// Constructs a new OutputStream from a byte array.
        /// </summary>
        /// <param name="buffer">Buffer to write bytes to</param>
        public OutputStream(byte[] buffer) : this(new MemoryStream(buffer)) { }

        /// <summary>
        /// Constructs a new OutputStream from an existing base stream.
        /// </summary>
        /// <param name="stream">Some generic stream type.</param>
        /// <exception cref="ArgumentNullException">When stream is null.</exception>
        public OutputStream(Stream stream)
        {
            jumpStack = new Stack<long>(16);
            fstream = stream ?? throw new ArgumentNullException(nameof(stream));
            buffer = new byte[defaultBufferSize];

            textBuffer = new byte[defaultTextBufferSize];
        }
    }
}