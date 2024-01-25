using System;
using System.IO;
using System.Collections.Generic;

 namespace OFC.IO
{
    public partial class InputStream : IDisposable
    {
        //Private Constants
        private const int defaultBufferSize = 16;

        //Private Data
        private readonly Stack<long> jumpStack;
        private Stream fstream;
        private readonly byte[] buffer;

        //Properties
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
        /// Constructs a new InputStream from a file.
        /// </summary>
        /// <param name="filepath">Path to the file</param>
        public InputStream(string filepath) : this(File.OpenRead(filepath)) { }

        /// <summary>
        /// Constructs a new InputStream from a byte array.
        /// </summary>
        /// <param name="buffer">Buffer to read bytes from</param>
        public InputStream(byte[] buffer) : this(new MemoryStream(buffer)) { }

        /// <summary>
        /// Constructs a new InputStream from an existing base stream
        /// </summary>
        /// <param name="stream">Some generic stream type.</param>
        /// <exception cref="ArgumentNullException">When stream is null.</exception>
        public InputStream(Stream stream)
        {
            jumpStack = new Stack<long>(16);
            fstream = stream ?? throw new ArgumentNullException(nameof(stream));
            buffer = new byte[defaultBufferSize];
        }
    }
}
