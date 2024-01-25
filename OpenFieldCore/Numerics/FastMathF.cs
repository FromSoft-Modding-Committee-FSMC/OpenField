namespace OFC.Numerics
{
    public class FastMathF
    {
        /// <summary>
        /// Constant for converting degrees into radians
        /// </summary>
        public const float DegRad = 0.01745329251f;

        /// <summary>
        /// Constant for converting radians into degrees
        /// </summary>
        public const float RadDeg = 57.2957795131f;

        /// <summary>
        /// Constant of PI
        /// </summary>
        public const float PI = 3.14159265359f;

        /// <summary>
        /// Constant of TAU
        /// </summary>
        public const float TAU = 6.28318530718f;

        // Trigonomerty

        /// <summary>
        /// A very fast/approximate calculation of sin using quadratic curves.
        /// <br>http://web.archive.org/web/20110925033606/http://lab.polygonal.de/2007/07/18/fast-and-accurate-sinecosine-approximation/</br>
        /// </summary>
        /// <param name="r">radians (between -PI and PI)</param>
        /// <returns>Approximation of sin</returns>
        public static float SinQ1(float r)
        {
            return 1.27323954474f * r - 0.40528473456f * r * System.MathF.Abs(r);
        }

        /// <summary>
        /// A slightly more accurate implementation of SinQ1.
        /// </summary>
        /// <param name="r">radians (between -PI and PI)</param>
        /// <returns>Approximation of sin</returns>
        public static float SinQ2(float r)
        {
            r = 1.27323954474f * r - 0.40528473456f * r * System.MathF.Abs(r);
            return 0.225f * (r * System.MathF.Abs(r) - r) + r;
        }
    }
}
