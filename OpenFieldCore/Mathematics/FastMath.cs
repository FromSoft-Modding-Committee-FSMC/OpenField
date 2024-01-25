using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.Mathematics
{
    public class FastMath
    {
        public static readonly float PI  = 3.141592655f;
        public static readonly float TAU = 6.28318531f;


        /// <summary>
        /// A very fast/approximate calculation of sin using quadratic curves.
        /// <br>http://web.archive.org/web/20110925033606/http://lab.polygonal.de/2007/07/18/fast-and-accurate-sinecosine-approximation/</br>
        /// </summary>
        /// <param name="x">angle in radians (between -PI and PI)</param>
        /// <returns>sin approx</returns>
        public static float Sin1(float x)
        {
            return 1.27323954474f * x - 0.40528473456f * x * MathF.Abs(x);
        }

        /// <summary>
        /// Produces more accurate approximations than Sin1
        /// <br>http://web.archive.org/web/20110925033606/http://lab.polygonal.de/2007/07/18/fast-and-accurate-sinecosine-approximation/</br>
        /// </summary>
        /// <param name="x">angle in radians (between -PI and PI)</param>
        /// <returns>sin approx</returns>
        public static float Sin2(float x)
        {
            x = 1.27323954474f * x - 0.40528473456f * x * MathF.Abs(x);
            return 0.225f * (x * MathF.Abs(x) - x) + x;
        }

        public static float Cos1(float x)
        {
            const float y = 1.0f / 6.28318531f;
            x *= y;
            x -= 0.25f + MathF.Floor(x + 0.25f);
            x *= 16.0f * (MathF.Abs(x) - 0.5f);

            return x;
        }
        public static float Cos2(float x)
        {
            const float y = 1.0f / 6.28318531f;
            x *= y;
            x -= 0.25f + MathF.Floor(x + 0.25f);
            x *= 16.0f * (MathF.Abs(x) - 0.5f);
            x += 0.225f * x * (MathF.Abs(x) - 1.0f);

            return x;
        }
        public unsafe static float Cos3(float x)
        {
            x += 1.5707963275f;
            bool b = (x > 3.141592655f);
            x -= (6.28318531f * *((byte*)&b));
            x = 1.27323954474f * x - 0.40528473456f * x * MathF.Abs(x);
            return 0.225f * (x * MathF.Abs(x) - x) + x;
        }
    }
}
