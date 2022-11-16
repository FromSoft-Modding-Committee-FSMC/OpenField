using System;
using System.Runtime.CompilerServices;

namespace OFC.Mathematics
{
    public struct Vector2s
    {
        //Data
        private readonly float[] _components;

        //Properties
        public static Vector2s Zero => new Vector2s(0f, 0f);
        public static Vector2s One => new Vector2s(1f, 1f);

        public float X
        {
            get
            {
                return _components[0];
            }
            set
            {
                _components[0] = value;
            }
        }
        public float Y
        {
            get
            {
                return _components[1];
            }
            set
            {
                _components[1] = value;
            }
        }

        //Constructors
        public Vector2s(float x, float y)
        {
            _components = new float[] {
                x,
                y
            };
        }
        public Vector2s(float[] components)
        {
            _components = components;
        }

        //Operators
        public override string ToString()
        {
            return $"x: {_components[0]}, y: {_components[1]}";
        }

        //Class Implementation
        public void Normalize()
        {
            float magnitudeInv = 1 / MathF.Sqrt(X * X + Y * Y);

            X *= magnitudeInv;
            Y *= magnitudeInv;
        }

        //Static Implementation
        /// <summary>
        /// Adds two vectors, storing the result in the first vector (a)
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref Vector2s a, ref Vector2s b)
        {
            a.X += b.X;
            a.Y += b.Y;
        }

        /// <summary>
        /// Adds two vectors, returning a new vector as a result
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2s Add(Vector2s a, Vector2s b)
        {
            return new Vector2s(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Subtracts two vectors, storing the result in the first vector (a)
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref Vector2s a, ref Vector2s b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
        }

        /// <summary>
        /// Subtracts two vectors, returning a new vector as a result
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2s Subtract(Vector2s a, Vector2s b)
        {
            return new Vector2s(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Divides two vectors, storing the result in the first vector (a)
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector2s a, ref Vector2s b)
        {
            a.X /= b.X;
            a.Y /= b.Y;
        }

        /// <summary>
        /// Divides two vectors, returning a new vector as a result
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2s Divide(Vector2s a, Vector2s b)
        {
            return new Vector2s(a.X / b.X, a.Y / b.Y);
        }

        /// <summary>
        /// Multiplies two vectors, storing the result in the first vector (a)
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector2s a, ref Vector2s b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
        }

        /// <summary>
        /// Multiplies two vectors, returning a new vector as a result
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2s Multiply(Vector2s a, Vector2s b)
        {
            return new Vector2s(a.X * b.X, a.Y * b.Y);
        }
        
        public static float Dot(ref Vector2s a, ref Vector2s b)
        {
            return (a.X * b.X + a.Y * b.Y);
        }

        public static Vector2s Project(Vector2s a, Vector2s b)
        {
            float k = Vector2s.Dot(ref a, ref b) / Vector2s.Dot(ref b, ref b);

            return new Vector2s(k * b.X, k * b.Y);
        }

        public static float EuclideanDistance(ref Vector2s a, ref Vector2s b)
        {
            return MathF.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

        public static float SquareDistance(ref Vector2s a, ref Vector2s b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }
    }
}
