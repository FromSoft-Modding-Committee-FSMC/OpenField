using OFC.Utility;

namespace OFC.Rendering
{
    public enum ColourSpace
    {
        RGBALinear = 0,
        HSVA       = 1,
    }

    public struct Colour
    {
        //Data
        private readonly float[] _components;
        private readonly ColourSpace colourSpace;
        private readonly uint _bytecomponents;

        //Properties
        public float R
        {
            get { return _components[0]; }
        }
        public float H
        {
            get { return _components[0]; }
        }

        public float G
        {
            get { return _components[1]; }
        }
        public float S
        {
            get { return _components[1]; }
        }
        public float C
        {
            get { return _components[1]; }
        }

        public float B
        {
            get { return _components[2]; }
        }
        public float V
        {
            get { return _components[2]; }
        }
        public float L
        {
            get { return _components[2]; }
        }

        public float A
        {
            get { return _components[3]; }
        }

        public uint AsInteger
        {
            get { return _bytecomponents; }
        }

        public System.Numerics.Vector4 AsNumericsVector
        {
            get
            {
                return new System.Numerics.Vector4(R, G, B, A);
            }
        }

        /// <summary>
        /// Constructs a new colour with a target colour space.
        /// </summary>
        /// <param name="colourSpace"></param>
        /// <param name="C1">First colour component, according to colour space (Red, Hue etc...)</param>
        /// <param name="C2">Second colour component, according to colour space (Green, Saturation)</param>
        /// <param name="C3">Third colour component, according to colour space (Blue, Value)</param>
        /// <param name="A">Alpha (transparency) of the colour</param>
        public Colour(ColourSpace colourSpace, float C1, float C2, float C3, float A)
        {
            _components = new float[]
            {
                C1,
                C2,
                C3,
                A
            };
            this.colourSpace = colourSpace;

            _bytecomponents = (uint)(((byte)(A * 255f) << 24) | ((byte)(C1 * 255f) << 16) | ((byte)(C2 * 255f) << 8) | ((byte)(C3 * 255f) << 0));
        }

        /// <summary>
        /// Constructs a new colour from floats, using RGBALinear colour space
        /// </summary>
        /// <param name="R">Red component</param>
        /// <param name="G">Green component</param>
        /// <param name="B">Blue component</param>
        /// <param name="A">Alpha (transparency) component</param>
        public Colour(float R, float G, float B, float A)
        {
            _components = new float[]
            {
                R,
                G,
                B,
                A
            };
            colourSpace = ColourSpace.RGBALinear;

            _bytecomponents = (uint)(((byte)(A * 255f) << 24) | ((byte)(B * 255f) << 16) | ((byte)(G * 255f) << 8) | ((byte)(R * 255f) << 0));
        }

        /// <summary>
        /// Constructs a new colour from bytes, using RGBALinear colour space
        /// </summary>
        /// <param name="R">Red component</param>
        /// <param name="G">Green component</param>
        /// <param name="B">Blue component</param>
        /// <param 
        public Colour(int R, int G, int B, int A)
        {
            _components = new float[]
            {
                R / 255f,
                G / 255f,
                B / 255f,
                A / 255f
            };
            colourSpace = ColourSpace.RGBALinear;

            _bytecomponents = (uint)(((byte)(A * 255f) << 24) | ((byte)(B * 255f) << 16) | ((byte)(G * 255f) << 8) | ((byte)(R * 255f) << 0));
        }

        /// <summary>
        /// Converts from the current colour space to another colour space
        /// </summary>
        /// <param name="to">Target colour space</param>
        public void ToColourSpace(ColourSpace to)
        {
            switch (to)
            {
                case ColourSpace.RGBALinear:
                    ToRGBALinear();
                    break;

                case ColourSpace.HSVA:
                    ToHSVA();
                    break;

                default:
                    Log.Warn($"Unknown destination colour space [{to}]");
                    break;
            }
        }

        /// <summary>
        /// Converts from the current colour space to RGBALinear colourspace
        /// </summary>
        public void ToRGBALinear()
        {
            switch (colourSpace)
            {
                default:
                    Log.Warn($"Unknown source colour space [{colourSpace}]");
                    return;
            }
        }

        /// <summary>
        /// Converts from the current colour space to HSV colour space
        /// </summary>
        public void ToHSVA()
        {
            switch(colourSpace)
            {
                default:
                    Log.Warn($"Unknown source colour space [{colourSpace}]");
                    return;
            }
        }
    }
}
