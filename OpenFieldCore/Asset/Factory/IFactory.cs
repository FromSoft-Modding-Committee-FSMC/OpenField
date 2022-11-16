using System;
using System.Collections.Generic;
using System.Text;

using OFC.Asset.Format;

namespace OFC.Asset.Factory
{
    public interface IFactory<T>
    {
        public List<string> EnumerateImportableFormats();
        public List<string> EnumerateExportableFormats();

        public bool RegisterHandler(IFormat<T> handler);

        public IFormat<T> GetHandler(int index);
        public IFormat<T> GetHandler(string name);
    }
}
