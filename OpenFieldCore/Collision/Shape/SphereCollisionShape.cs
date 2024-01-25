using OFC.Numerics;

namespace OFC.Collision.Shape
{
    public class SphereCollisionShape : ICollisionShape
    {
        public Vector2f origin = new Vector2f(0f, 0f);
        public float radius = 16f;

        public bool LineIntersects(Vector2f p1, Vector2f p2)
        {
            return false;
        }
    }
}
