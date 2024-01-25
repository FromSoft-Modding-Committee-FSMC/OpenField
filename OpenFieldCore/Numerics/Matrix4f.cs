using System;
using System.Runtime.CompilerServices;

namespace OFC.Numerics
{
    public readonly struct Matrix4f
    {
        //Data
        readonly float[] components;

        //Properties
        public float[] Components => components;

        //Constructors
        /// <summary>
        /// A constructor with no parameters makes an identity matrix
        /// </summary>
        public Matrix4f()
        {
            components = new float[]
            {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            };
        }

        /// <summary>
        /// A constructor which creates a matrix from raw float values
        /// </summary>
        /// <param name="components"></param>
        public Matrix4f(float[] components)
        {
            this.components = components;
        }

        public static Matrix4f CreateLookAt(Vector3f from, Vector3f to, Vector3f up)
        {
            Matrix4f result = new();

            Vector3f zAxis = Vector3f.Normalized(to - from);
            Vector3f xAxis = Vector3f.Normalized(Vector3f.Cross(up, zAxis));
            Vector3f yAxis = Vector3f.Cross(zAxis, xAxis);

            result.components[00] = xAxis.X;
            result.components[01] = yAxis.X;
            result.components[02] = zAxis.X;
            result.components[04] = xAxis.Y;
            result.components[05] = yAxis.Y;
            result.components[06] = zAxis.Y;
            result.components[08] = xAxis.Z;
            result.components[09] = yAxis.Z;
            result.components[10] = zAxis.Z;
            result.components[12] = -Vector3f.Dot(xAxis, from);
            result.components[13] = -Vector3f.Dot(yAxis, from);
            result.components[14] = -Vector3f.Dot(zAxis, from);

            return result;
        }

        /// <summary>
        /// Creates an off centre orthographic matrix
        /// </summary>
        /// <param name="x1">Left boundry</param>
        /// <param name="y1">Top boundry</param>
        /// <param name="x2">Right boundry</param>
        /// <param name="y2">Bottom boundry</param>
        /// <param name="near">Negative depth</param>
        /// <param name="far">Positive depth</param>
        /// <returns>The created matrix</returns> 
        public static Matrix4f CreateOrthographic(float x1, float y1, float x2, float y2, float near, float far)
        {
            //Create an identity matrix as our basic result.
            Matrix4f result = new();
            result.components[00] = 2f / (x2 - x1);
            result.components[05] = 2f / (y1 - y2);
            result.components[10] = -2f / (far - near);
            result.components[12] = -((x2 + x1) / (x2 - x1));
            result.components[13] = -((y1 + y2) / (y1 - y2));
            result.components[14] = -((far + near) / (far - near));
            return result;
        }


        public static Matrix4f CreatePerspective(float fov, float aspect, float near, float far)
        {
            Matrix4f result = new(new float[] {0,0,0,0, 0,0,0,0, 0,0,0,1, 0,0,0,0 });

            float yscale = 1f / MathF.Tan((fov * 0.0174533f) / 2f);
            float xscale = yscale / aspect;

            result.components[00] = xscale;
            result.components[05] = yscale;
            result.components[10] = far / (far - near);
            result.components[14] = -near * far / (far - near);

            return result;
        }


        public static Matrix4f CreateRotation(Vector3f axis, float angle)
        {
            Matrix4f result = new();

            //Rotation terms
            float C = MathF.Cos(angle);
            float S = MathF.Sin(angle);

            result.components[0]  = axis.X * axis.X * (1f - C) + C;
            result.components[1]  = axis.X * axis.Y * (1f - C) - axis.Z * S;
            result.components[2]  = axis.X * axis.Z * (1f - C) + axis.Y * S;
            result.components[4]  = axis.Y * axis.X * (1f - C) + axis.Z * S;
            result.components[5]  = axis.Y * axis.Y * (1f - C) + C;
            result.components[6]  = axis.Y * axis.Z * (1f - C) - axis.X * S;
            result.components[8]  = axis.Z * axis.X * (1f - C) - axis.Y * S;
            result.components[9]  = axis.Z * axis.Y * (1f - C) + axis.X * S;
            result.components[10] = axis.Z * axis.Z * (1f - C) + C;

            return result;
        }


        public static Matrix4f CreateRotationY(float angle)
        {
            Matrix4f result = new();

            //Rotation Terms
            float C = MathF.Cos(angle);
            float S = MathF.Sin(angle);

            result.components[00] = C;
            result.components[02] = S;
            result.components[08] = -S;
            result.components[10] = C;

            return result;
        }


        public static Matrix4f CreateTranslation(float x, float y, float z)
        {
            //Create an identity matrix as our basic result.
            Matrix4f result = new();
            result.components[12] = x;
            result.components[13] = y;
            result.components[14] = z;
            return result;
        }


        public static Matrix4f CreateTranslation(Vector3f xyz)
        {
            Matrix4f result = new();
            result.components[12] = xyz.X;
            result.components[13] = xyz.Y;
            result.components[14] = xyz.Z;
            return result;
        }

        // Operator Overloads
        public static Matrix4f operator *(Matrix4f lhs, Matrix4f rhs)
        {
            Matrix4f result = new Matrix4f();

            #region Row 1
            result.components[0] = 
                lhs.components[0] * rhs.components[0] + 
                lhs.components[1] * rhs.components[4] + 
                lhs.components[2] * rhs.components[8] + 
                lhs.components[3] * rhs.components[12];

            result.components[1] = 
                lhs.components[0] * rhs.components[1] + 
                lhs.components[1] * rhs.components[5] + 
                lhs.components[2] * rhs.components[9] + 
                lhs.components[3] * rhs.components[13];

            result.components[2] = 
                lhs.components[0] * rhs.components[2] + 
                lhs.components[1] * rhs.components[6] + 
                lhs.components[2] * rhs.components[10] + 
                lhs.components[3] * rhs.components[14];

            result.components[3] = 
                lhs.components[0] * rhs.components[3] + 
                lhs.components[1] * rhs.components[7] + 
                lhs.components[2] * rhs.components[11] + 
                lhs.components[3] * rhs.components[15];
            #endregion
            #region Row 2
            result.components[4] = 
                lhs.components[4] * rhs.components[0] + 
                lhs.components[5] * rhs.components[4] + 
                lhs.components[6] * rhs.components[8] + 
                lhs.components[7] * rhs.components[12];

            result.components[5] =
                lhs.components[4] * rhs.components[1] +
                lhs.components[5] * rhs.components[5] +
                lhs.components[6] * rhs.components[9] +
                lhs.components[7] * rhs.components[13];

            result.components[6] =
                lhs.components[4] * rhs.components[2] +
                lhs.components[5] * rhs.components[6] +
                lhs.components[6] * rhs.components[10] +
                lhs.components[7] * rhs.components[14];

            result.components[7] =
                lhs.components[4] * rhs.components[3] +
                lhs.components[5] * rhs.components[7] +
                lhs.components[6] * rhs.components[11] +
                lhs.components[7] * rhs.components[15];
            #endregion
            #region Row 3
            result.components[8] =
                lhs.components[8]  * rhs.components[0] +
                lhs.components[9]  * rhs.components[4] +
                lhs.components[10] * rhs.components[8] +
                lhs.components[11] * rhs.components[12];

            result.components[9] =
                lhs.components[8] * rhs.components[1] +
                lhs.components[9] * rhs.components[5] +
                lhs.components[10] * rhs.components[9] +
                lhs.components[11] * rhs.components[13];

            result.components[10] =
                lhs.components[8] * rhs.components[2] +
                lhs.components[9] * rhs.components[6] +
                lhs.components[10] * rhs.components[10] +
                lhs.components[11] * rhs.components[14];

            result.components[11] =
                lhs.components[8] * rhs.components[3] +
                lhs.components[9] * rhs.components[7] +
                lhs.components[10] * rhs.components[11] +
                lhs.components[11] * rhs.components[15];
            #endregion
            #region Row 4
            result.components[12] =
                lhs.components[12] * rhs.components[0] +
                lhs.components[13] * rhs.components[4] +
                lhs.components[14] * rhs.components[8] +
                lhs.components[15] * rhs.components[12];

            result.components[13] =
                lhs.components[12] * rhs.components[1] +
                lhs.components[13] * rhs.components[5] +
                lhs.components[14] * rhs.components[9] +
                lhs.components[15] * rhs.components[13];

            result.components[14] =
                lhs.components[12] * rhs.components[2] +
                lhs.components[13] * rhs.components[6] +
                lhs.components[14] * rhs.components[10] +
                lhs.components[15] * rhs.components[14];

            result.components[15] =
                lhs.components[12] * rhs.components[3] +
                lhs.components[13] * rhs.components[7] +
                lhs.components[14] * rhs.components[11] +
                lhs.components[15] * rhs.components[15];
            #endregion

            return result;
        }
    }
}
