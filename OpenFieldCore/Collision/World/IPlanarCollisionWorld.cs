using OFC.Mathematics;

namespace OFC.Collision.World
{
    public struct PlanarTraceResult
    {
        public Vector2s intersectionPoint;
        public float intersectionDistance;
    } 

    public interface IPlanarCollisionWorld
    {
        public bool LineTrace(Vector2s start, Vector2s direction, float maxDistance, uint mask, out PlanarTraceResult result);
    }
}
