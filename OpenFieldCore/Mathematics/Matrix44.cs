using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.Mathematics
{
    public struct Matrix44
    {
        //Data
        private float[] _components;

        //Properties
        public float[] Components
        {
            get
            {
                return _components;
            }
        }

        public static Matrix44 Identity
        {
            get
            {
                return new Matrix44(new float[] 
                {
                    1f, 0f, 0f, 0f,
                    0f, 1f, 0f, 0f,
                    0f, 0f, 1f, 0f,
                    0f, 0f, 0f, 1f
                });
            }
        }

        //Constructors
        public Matrix44(float[] components)
        {
            _components = components;
        }

        public static Matrix44 CreateOrthographic(float x1, float y1, float x2, float y2, float znear, float zfar)
        {
            Matrix44 result = Identity;

            result._components[0] = 2 / (x2 - x1);
            result._components[5] = 2 / (y1 - y2);
            result._components[10] = -2 / (zfar - znear);

            result._components[12] = -(x2 + x1) / (x2 - x1);
            result._components[13] = -(y1 + y2) / (y1 - y2);
            result._components[14] = -(zfar + znear) / (zfar - znear);

            return result;
        }
    }
}