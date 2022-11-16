using System;
using System.Runtime.CompilerServices;

namespace OFC.Mathematics
{
    public struct Vector2i
    {
        //Data
        private int[] _components;

        //Properties
        public static Vector2i Zero => new Vector2i(0, 0);
        public static Vector2i One => new Vector2i(1, 1);

        public int X
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
        public int Y
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
        public Vector2i(int x, int y)
        {
            _components = new int[] {
                x,
                y
            };
        }
        public Vector2i(int[] components)
        {
            _components = components;
        }

        //Operators
        public override string ToString()
        {
            return $"x: {_components[0]}, y: {_components[1]}";
        }

    }
}
