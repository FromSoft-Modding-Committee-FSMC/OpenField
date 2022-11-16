using System;
using System.Runtime.InteropServices;

namespace OFC.Utility
{
    public delegate void LogOutput(string message);

    public static class Log
    {
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private static bool useConsoleColour = true;

        private static readonly string logHeadingStart = "<".Colourize(0x444444) + "[".Colourize(0xAAAAAA);
        private static readonly string logHeadingEnd = "]".Colourize(0xAAAAAA) + ">".Colourize(0x444444) + ": ".Colourize(0x444444);
        private static readonly string logHeadingCenter = "]".Colourize(0xAAAAAA) + "-".Colourize(0x888888) + "[".Colourize(0xAAAAAA);

        private static LogOutput outputDelegate = s => { Console.WriteLine(s); };

        static Log()
        {
            useConsoleColour &= (Environment.GetEnvironmentVariable("NO_COLOR") == null);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr iStdOut = GetStdHandle(-11);

                useConsoleColour &= GetConsoleMode(iStdOut, out var outConsoleMode);
                useConsoleColour &= SetConsoleMode(iStdOut, outConsoleMode | 0x0004);
            }
        }

        public static void Write(string heading, uint headingColour, string message)
        {
            DateTime dt = DateTime.Now;

            if (useConsoleColour)
            {
                outputDelegate(string.Join("",
                    logHeadingStart,
                    $"{dt.Day:D2}:{dt.Month:D2}:{dt.Year:D4}".Colourize(0xCCCCCC),
                    logHeadingCenter,
                    $"{dt.Hour:D2}:{dt.Minute:D2}:{dt.Second:D2}",
                    logHeadingCenter,
                    heading.Colourize(headingColour),
                    logHeadingEnd,
                    message.Colourize(0xFFFFFF)
                    ));
            }
            else
            {
                outputDelegate(string.Join("",
                    "<[",
                    $"{dt.Day:D2}:{dt.Month:D2}:{dt.Year:D4}",
                    "]-[",
                    $"{dt.Hour:D2}:{dt.Minute:D2}:{dt.Second:D2}",
                    "]-[",
                    heading,
                    "]>: ",
                    message
                    ));
            }
        }

        public static void Info(string message)
        {
            Write("INFO", 0x88CCFF, message);
        }
        public static void Warn(string message)
        {
            Write("WARN", 0xFFFF88, message);
        }
        public static void Error(string message)
        {
            Write("SHIT", 0xFF8888, message);
        }

        public static void SetOutputDelegate(LogOutput outputDelegate)
        {
            Log.outputDelegate = outputDelegate;
        }

        public static void EnableColour(bool enabled)
        {
            useConsoleColour = enabled;
        }

        private static string Colourize(this string input, uint colour)
        {
            return $"\u001b[38;2;{(colour >> 16) & 0xFF};{(colour >> 8) & 0xFF};{(colour >> 0) & 0xFF}m{input}\u001b[0m";
        }
    }
}
