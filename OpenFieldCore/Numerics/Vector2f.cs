using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Numerics
{
    public struct Vector2f
    {
        // Vector Data Storage
        float[] components = new float[2];

        // Vector Properties
        public float X
        {
            get { return components[0]; }
            set { components[0] = value; }
        }
        public float Y
        {
            get { return components[1]; }
            set { components[1] = value; }
        }

        // Constructors
        public Vector2f(float x, float y)
        {
            components[0] = x;
            components[1] = y;
        }

        // Operations
        public static Vector2f Normalized(Vector2f v)
        {
            float magnitudeInv = v.MagnitudeInv();

            return new Vector2f(v.X * magnitudeInv, v.Y * magnitudeInv);
        }
        public static float Distance(Vector2f A, Vector2f B)
        {
            return MathF.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
        }
        public static float DistanceSquare(Vector2f A, Vector2f B)
        {
            return (B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y);
        }

        // Self Operations
        public void Normalize()
        {
            float magnitudeInv = MagnitudeInv();
            X *= magnitudeInv;
            Y *= magnitudeInv;
        }
        public float Magnitude()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }
        public float MagnitudeSquare()
        {
            return X * X + Y * Y;
        }
        public float MagnitudeInv()
        {
            return 1 / MathF.Sqrt(X * X + Y * Y);
        }

        // Simple Operators
        public static Vector2f operator +(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
        public static Vector2f operator +(Vector2f lhs, float rhs)
        {
            return new Vector2f(lhs.X + rhs, lhs.Y + rhs);
        }
        public static Vector2f operator -(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
        public static Vector2f operator -(Vector2f lhs, float rhs)
        {
            return new Vector2f(lhs.X - rhs, lhs.Y - rhs);
        }
        public static Vector2f operator *(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }
        public static Vector2f operator *(Vector2f lhs, float rhs)
        {
            return new Vector2f(lhs.X * rhs, lhs.Y * rhs);
        }
        public static Vector2f operator /(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.X / rhs.X, lhs.Y / rhs.Y);
        }
        public static Vector2f operator /(Vector2f lhs, float rhs)
        {
            return new Vector2f(lhs.X / rhs, lhs.Y / rhs);
        }

        // Equality Operators
        public static bool operator ==(Vector2f lhs, Vector2f rhs)
        {
            return (lhs.X == rhs.X) & (lhs.Y == rhs.Y);
        }
        public static bool operator !=(Vector2f lhs, Vector2f rhs)
        {
            return (lhs.X != rhs.X) | (lhs.Y != rhs.Y);
        }

        // Equality Overrides
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
