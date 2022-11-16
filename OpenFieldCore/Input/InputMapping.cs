namespace OFC.Input
{
    public enum InputMappingType
    {
        Button,
        Axis,
    };

    public struct InputMapping
    {
        public InputMappingType mappingType;
        public Control[] controls;
        public float axisScale;
    }
}
