using System;
using System.IO;

namespace OFC.IO
{
    public partial class InputStream
    {
        protected void Dispose(bool disposeManagedObjects)
        {
            if (disposeManagedObjects)
            {
                //Clear jumpstack because paranoid
                jumpStack.Clear();

                //Dispose internal stream
                fstream.Dispose();
                fstream = Stream.Null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        ~InputStream()
        {
            Dispose(false);
        }
    }
}
