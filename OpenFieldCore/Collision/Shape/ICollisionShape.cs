using OFC.Numerics;

namespace OFC.Collision.Shape
{
    public interface ICollisionShape
    {
        public bool LineIntersects(Vector2f p1, Vector2f p2);
    }
}
