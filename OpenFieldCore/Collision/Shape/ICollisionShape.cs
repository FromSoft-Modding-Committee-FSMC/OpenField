using OFC.Mathematics;

namespace OFC.Collision.Shape
{
    public interface ICollisionShape
    {
        public bool LineIntersects(Vector2s p1, Vector2s p2);
    }
}
