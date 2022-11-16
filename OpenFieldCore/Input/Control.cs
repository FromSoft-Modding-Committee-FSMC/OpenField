namespace OFC.Input
{
    public struct Control
    {
        public InputDeviceType deviceType;
        public Input[] inputs;
        public float axisScale;

        public static Control Axis(InputDeviceType deviceType, Input axis, float axisScale)
        {
            return new Control
            {
                deviceType = deviceType,
                axisScale = axisScale,
                inputs = new Input[]
                {
                axis
                }
            };
        }
        public static Control Axis(InputDeviceType deviceType, Input axisN, Input axisP, float axisScale)
        {
            return new Control
            {
                deviceType = deviceType,
                axisScale = axisScale,
                inputs = new Input[]
                {
                axisN,
                axisP
                }
            };
        }
        public static Control Button(InputDeviceType deviceType, Input button)
        {
            return new Control
            {
                deviceType = deviceType,
                inputs = new Input[]
                {
                button
                }
            };
        }

    }
}
