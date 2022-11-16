using System.Collections.Generic;

using OFC.Utility;

namespace OpenFieldForge.Asset
{
    public class NameTableAsset
    {
        private Dictionary<string, string> nameTable;

        public string GetName(string hashKey)
        {
            if (!nameTable.TryGetValue(hashKey, out string result))
            {
                Log.Warn($"Couldn't fild name for hash: {hashKey}");
                return null;
            }

            return result;
        }
    }
}
