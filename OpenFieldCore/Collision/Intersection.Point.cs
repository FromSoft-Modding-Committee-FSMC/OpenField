using OFC.Collision.Result;
using OFC.Numerics;

namespace OFC.Collision
{
    public static partial class IntersectionHelper
    {
        /// <summary>
        /// Checks if two points are identical
        /// </summary>
        /// <param name="A">The first point</param>
        /// <param name="B">The second point</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D PointPoint(Vector3f pointA, Vector3f pointB)
        {
            return new SIntersectionResult3D
            {
                intersects = (pointA == pointB),
                point = pointA
            };
        }


        /// <summary>
        /// Checks if a point is on a plane
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="planeOrigin">The origin of the plane</param>
        /// <param name="planeNormal">The normal of the plane</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D PointPlane(Vector3f point, Vector3f planeOrigin, Vector3f planeNormal)
        {
            //Find the closest point on the plane
            Vector3f closestPoint = point - Vector3f.Dot(planeNormal, (point - planeOrigin)) * planeNormal;

            return new SIntersectionResult3D
            {
                intersects = Vector3f.Distance(point, closestPoint) < 0.001, //We allow a small amount of error
                point = point
            };
        }


        /// <summary>
        /// Checks if a point is inside a sphere
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="sphereOrigin">The origin on the sphere</param>
        /// <param name="sphereRadius">The radius of the sphere</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D PointSphere(Vector3f point, Vector3f sphereOrigin, float sphereRadius)
        {
            return new SIntersectionResult3D
            {
                intersects = (Vector3f.Distance(point, sphereOrigin) <= sphereRadius),
                point = point
            };
        }


        /// <summary>
        /// Checks if a point is inside an axis aligned cube
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="cubeOrigin">The origin of the cube</param>
        /// <param name="cubeHalfSizes">The half-size of the cube on each axis</param>
        /// <returns>The intersection result</returns>
        public static SIntersectionResult3D PointCube(Vector3f point, Vector3f cubeOrigin, Vector3f cubeHalfSizes)
        {
            return new SIntersectionResult3D
            {
                intersects = 
                    (point.X > cubeOrigin.X - cubeHalfSizes.X) & (point.Y < cubeOrigin.X + cubeHalfSizes.X) &
                    (point.Y > cubeOrigin.Y - cubeHalfSizes.Y) & (point.Y < cubeOrigin.Y + cubeHalfSizes.Y) &
                    (point.Z > cubeOrigin.Z - cubeHalfSizes.Z) & (point.Z < cubeOrigin.Z + cubeHalfSizes.Z),
                point = point
            };
        }
    }
}
