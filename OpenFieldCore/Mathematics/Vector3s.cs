using System;
using System.Runtime.CompilerServices;

namespace OFC.Mathematics
{
    public struct Vector3s
    {
        //Data
        private float[] _components;

        //Properties
        public static Vector3s Zero => new Vector3s(0f, 0f, 0f);
        public static Vector3s One => new Vector3s(1f, 1f, 1f);

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

        //Constructors
        public Vector3s(float x, float y, float z)
        {
            _components = new float[] {
                x,
                y,
                z
            };
        }
        public Vector3s(float[] components)
        {
            _components = components;
        }

        //Operators
        public override string ToString()
        {
            return $"x: {_components[0]}, y: {_components[1]}, z: {_components[2]}";
        }
    }
}
