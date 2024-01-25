using OFC.Collision.Result;
using OFC.Numerics;
using System.Drawing;

namespace OFC.Collision
{
    public static partial class IntersectionHelper
    {
        /// <summary>
        /// Checks if a sphere and plane intersect
        /// </summary>
        /// <param name="sphereOrigin">The origin of the sphere</param>
        /// <param name="sphereRadius">The radius of the sphere</param>
        /// <param name="planeOrigin">The origin of the plane</param>
        /// <param name="planeNormal">The normal of the plane</param>
        /// <returns></returns>
        public static SIntersectionResult3D SpherePlane(Vector3f sphereOrigin, float sphereRadius, Vector3f planeOrigin, Vector3f planeNormal)
        {
            //Find the closest point on the plane
            Vector3f closestPoint = sphereOrigin - Vector3f.Dot(planeNormal, (sphereOrigin - planeOrigin)) * planeNormal;

            return new SIntersectionResult3D
            {
                intersects = Vector3f.DistanceSquare(closestPoint, sphereOrigin) <= (sphereRadius * sphereRadius),
                point = Vector3f.Zero
            };
        }

        /// <summary>
        /// Checks if two spheres intersect
        /// </summary>
        /// <param name="sphereAOrigin">The origin of the first sphere</param>
        /// <param name="sphereARadius">The radius of the first sphere</param>
        /// <param name="sphereBOrigin">The origin of the second sphere</param>
        /// <param name="sphereBRadius">The radius of the second sphere</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D SphereSphere(Vector3f sphereAOrigin, float sphereARadius, Vector3f sphereBOrigin, float sphereBRadius)
        {
            float D = Vector3f.DistanceSquare(sphereAOrigin, sphereBOrigin);

            return new SIntersectionResult3D
            {
                intersects = (D <= ((sphereARadius * sphereARadius) + (sphereBRadius * sphereBRadius))),
                point = Vector3f.Zero
            };
        }


        /// <summary>
        /// Checks if a sphere and cube intersect
        /// </summary>
        /// <param name="sphereOrigin">The origin of the sphere</param>
        /// <param name="sphereRadius">The radius of the sphere</param>
        /// <param name="cubeOrigin">The origin of the cube</param>
        /// <param name="cubeHalfSizes">The half-size of the cube on each axis</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D SphereCube(Vector3f sphereOrigin, float sphereRadius, Vector3f cubeOrigin, Vector3f cubeHalfSizes)
        {
            //Calculate cube min - max
            Vector3f cubeMin = cubeOrigin - cubeHalfSizes;
            Vector3f cubeMax = cubeOrigin + cubeHalfSizes;

            // Find square distance to the closest point of the cube - can faster pls?
            float DSquare = 0f;
            if (sphereOrigin.X < cubeMin.X) { DSquare += (cubeMin.X - sphereOrigin.X) * (cubeMin.X - sphereOrigin.X); }
            if (sphereOrigin.X > cubeMax.X) { DSquare += (sphereOrigin.X - cubeMax.X) * (sphereOrigin.X - cubeMax.X); }
            if (sphereOrigin.Y < cubeMin.Y) { DSquare += (cubeMin.Y - sphereOrigin.Y) * (cubeMin.Y - sphereOrigin.Y); }
            if (sphereOrigin.Y > cubeMax.Y) { DSquare += (sphereOrigin.Y - cubeMax.Y) * (sphereOrigin.Y - cubeMax.Y); }
            if (sphereOrigin.Z < cubeMin.Z) { DSquare += (cubeMin.Z - sphereOrigin.Z) * (cubeMin.Z - sphereOrigin.Z); }
            if (sphereOrigin.Z > cubeMax.Z) { DSquare += (sphereOrigin.Z - cubeMax.Z) * (sphereOrigin.Z - cubeMax.Z); }

            return new SIntersectionResult3D
            {
                intersects = (DSquare <= (sphereRadius * sphereRadius)),
                point = Vector3f.Zero
            };
        }
    }
}
