using OFC.Collision.Result;
using OFC.Numerics;

namespace OFC.Collision
{
    public static partial class IntersectionHelper
    {
        public static SIntersectionResult3D CubeCube(Vector3f cubeAOrigin, Vector3f cubeAHalfSizes, Vector3f cubeBOrigin, Vector3f cubeBHalfSizes)
        {
            Vector3f minA = cubeAOrigin - cubeAHalfSizes;
            Vector3f maxA = cubeAOrigin + cubeAHalfSizes;
            Vector3f minB = cubeBOrigin - cubeBHalfSizes;
            Vector3f maxB = cubeBOrigin + cubeBHalfSizes;

            return new SIntersectionResult3D
            {
                intersects =
                    minA.X <= maxB.X && maxA.X >= minB.X &&
                    minA.Y <= maxB.Y && maxA.Y >= minB.Y &&
                    minA.Z <= maxB.Z && maxA.Z >= minB.Z,
                point = Vector3f.Zero
            };
        }
    }
}
