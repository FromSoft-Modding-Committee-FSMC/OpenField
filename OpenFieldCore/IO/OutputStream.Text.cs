using System;
using System.Text;

namespace OFC.IO
{
    public partial class OutputStream
    {
        public void Write(string v)
        {
            byte[] chars = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(v));
            int charPos = 0;

            while (charPos < chars.Length)
            {
                //Copy 8B
                while ((chars.Length - charPos) >= 8 && (textPos+8 < defaultTextBufferSize))
                {
                    Array.Copy(chars, charPos, textBuffer, textPos, 8);
                    charPos += 8;
                    textPos += 8;
                }

                //Copy 1B
                while (((chars.Length - charPos) >= 1) && (textPos < defaultTextBufferSize))
                    textBuffer[textPos++] = chars[charPos++];

                //Flush when required.
                if(textPos >= defaultTextBufferSize)
                    WriteFlush();
            }
        }
        public void WriteLine(string v)
        {
            Write($"{v}{Environment.NewLine}");
        }
        public void WriteLines(string[] v)
        { 
            foreach(string line in v)
                WriteLine(line);
        }
        public void WriteFlush()
        {
            fstream.Write(textBuffer, 0, textPos);
            textPos = 0;
        }
    }
}
