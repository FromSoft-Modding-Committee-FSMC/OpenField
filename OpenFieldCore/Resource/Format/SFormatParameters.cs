namespace OFC.Resource.Format
{
    public delegate bool ValidateImportFile(byte[] buffer);

    public struct SFormatParameters
    {
        public SFormatMetadata metadata;

        public bool allowExport;
        public bool allowImport;
        public ValidateImportFile validator;
    }
}
