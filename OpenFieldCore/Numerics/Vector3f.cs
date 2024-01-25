using System;

namespace OFC.Numerics
{
    public struct Vector3f
    {
        // Defaults
        public static readonly Vector3f Zero = new(0, 0, 0);
        public static readonly Vector3f UnitX = new(1, 0, 0);
        public static readonly Vector3f UnitY = new(0, 1, 0);
        public static readonly Vector3f UnitZ = new(0, 0, 1);

        // CPU Data
        float[] components;

        // Properties
        public float X { get => components[0]; set => components[0] = value; }
        public float Y { get => components[1]; set => components[1] = value; }
        public float Z { get => components[2]; set => components[2] = value; }

        // Constructors
        public Vector3f() : this(0, 0, 0) { }
        public Vector3f(float x, float y, float z)
        {
            components = new float[]
            {
                x,
                y,
                z
            };
        }

        // Operations
        public static Vector3f Normalized(Vector3f v)
        {
            float magnitudeInv = v.MagnitudeInv();

            return new Vector3f(v.X * magnitudeInv, v.Y * magnitudeInv, v.Z * magnitudeInv);
        }
        public static Vector3f Cross(Vector3f A, Vector3f B)
        {
            Vector3f result = new Vector3f();
            result.X = A.Y * B.Z - A.Z * B.Y;
            result.Y = A.Z * B.X - A.X * B.Z;
            result.Z = A.X * B.Y - A.Y * B.X;
            return result;
        }

        public static float Dot(Vector3f A, Vector3f B)
        {
            return A.X * B.X + A.Y * B.Y + A.Z * B.Z;
        }
        public static float Distance(Vector3f A, Vector3f B)
        {
            float ABX = B.X - A.X, ABY = B.Y - A.Y, ABZ = B.Z - A.Z;

            return MathF.Sqrt(ABX * ABX + ABY * ABY + ABZ * ABZ);
        }
        public static float DistanceSquare(Vector3f A, Vector3f B)
        {
            return (B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y) + (B.Z - A.Z) * (B.Z - A.Z);
        }

        // Self Operations
        public void Normalize()
        {
            float magnitudeInv = MagnitudeInv();
            X *= magnitudeInv;
            Y *= magnitudeInv;
            Z *= magnitudeInv;
        }
        public float Magnitude()
        {
            return MathF.Sqrt(X * X + Y * Y + Z * Z);
        }
        public float MagnitudeSquare()
        {
            return X * X + Y * Y + Z * Z;
        }
        public float MagnitudeInv()
        {
            return 1 / MathF.Sqrt(X * X + Y * Y + Z * Z);
        }

        // Simple Operators
        public static Vector3f operator +(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }
        public static Vector3f operator +(Vector3f lhs, float rhs)
        {
            return new Vector3f(lhs.X + rhs, lhs.Y + rhs, lhs.Z + rhs);
        }
        public static Vector3f operator -(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }
        public static Vector3f operator -(Vector3f lhs, float rhs)
        {
            return new Vector3f(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs);
        }
        public static Vector3f operator *(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }
        public static Vector3f operator *(Vector3f lhs, float rhs)
        {
            return new Vector3f(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }
        public static Vector3f operator *(float lhs, Vector3f rhs)
        {
            return new Vector3f(lhs * rhs.X, lhs * rhs.Y, lhs * rhs.Z);
        }

        public static Vector3f operator /(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z);
        }
        public static Vector3f operator /(Vector3f lhs, float rhs)
        {
            return new Vector3f(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        // Equality Operators
        public static bool operator ==(Vector3f lhs, Vector3f rhs)
        {
            return (lhs.X == rhs.X) & (lhs.Y == rhs.Y) & (lhs.Z == rhs.Z);
        }
        public static bool operator !=(Vector3f lhs, Vector3f rhs)
        {
            return (lhs.X != rhs.X) | (lhs.Y != rhs.Y) | (lhs.Z != rhs.Z);
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
