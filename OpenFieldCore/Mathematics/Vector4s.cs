using System;
using System.Runtime.CompilerServices;

namespace OFC.Mathematics
{
    public struct Vector4s
    {
        //Data
        private float[] _components;

        //Properties
        public static Vector4s Zero => new Vector4s(0f, 0f, 0f, 0f);
        public static Vector4s One => new Vector4s(1f, 1f, 1f, 1f);

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
        public float Z
        {
            get
            {
                return _components[2];
            }
            set
            {
                _components[2] = value;
            }
        }
        public float W
        {
            get
            {
                return _components[3];
            }
            set
            {
                _components[3] = value;
            }
        }

        //Constructors
        public Vector4s(float x, float y, float z, float w)
        {
            _components = new float[] {
                x,
                y,
                z,
                w
            };
        }
        public Vector4s(float[] components)
        {
            _components = components;
        }

        //Operators
        public override string ToString()
        {
            return $"x: {_components[0]}, y: {_components[1]}, z: {_components[2]}, w: {_components[3]}";
        }
    }
}
