using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.Asset.Format
{
    public delegate bool ValidateFile(byte[] buffer);

    public struct FormatParameters
    {
        public ValidateFile validator;
        public FormatType type;
        public bool allowExport;
        public bool allowImport;
        public string filter;
        public string name;
        public string description;
        public string[] extensions;
    }
}
