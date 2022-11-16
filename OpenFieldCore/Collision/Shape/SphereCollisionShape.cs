using System;
using System.Collections.Generic;
using System.Text;

using OFC.Mathematics;

namespace OFC.Collision.Shape
{
    public class SphereCollisionShape : ICollisionShape
    {
        public Vector2s origin = Vector2s.Zero;
        public float radius = 16f;

        public bool LineIntersects(Vector2s p1, Vector2s p2)
        {
            return false;
        }
    }
}
