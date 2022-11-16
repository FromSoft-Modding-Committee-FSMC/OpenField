using System;
using System.Collections.Generic;
using System.Text;

namespace OFC.Asset.Format
{
    public interface IFormat<T>
    {
        FormatParameters Parameters { get; }

        bool Load(string filepath, out T asset);
        bool Load(byte[] buffer, out T asset);

        void Save(string filepath, T asset);
    }
}
