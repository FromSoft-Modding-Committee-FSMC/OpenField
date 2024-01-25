using System;
using System.Collections.Generic;
using System.Linq;

using OFC.Resource.Format;

namespace OFC.Resource.Factory
{
    public abstract class FormatFactory<T>
    {
        protected List<IFormat<T>> registeredFormats = new();

        /// <summary>
        /// Enumurates all registered format handlers which are capable of exporting a file.
        /// </summary>
        /// <returns>A list of format metadata</returns>
        public List<SFormatMetadata> EnumerateFormats(EFormatFilter filter = EFormatFilter.None)
        {
            List<SFormatMetadata> metaData = new();
            foreach (IFormat<T> fmt in registeredFormats)
            {
                //Don't Add if the filter doesn't pass
                if (!fmt.Parameters.allowExport && (filter & EFormatFilter.Exportable) > 0)
                    continue;

                if (!fmt.Parameters.allowImport && (filter & EFormatFilter.Importable) > 0)
                    continue;

                metaData.Add(fmt.Parameters.metadata);
            }

            return metaData;
        }

        /// <summary>
        /// Registers a format handler.
        /// </summary>
        /// <param name="fileFormat">A file format handler</param>
        /// <returns>True on success</returns>
        public virtual bool RegisterFormat(IFormat<T> fileFormat)
        {
            registeredFormats.Add(fileFormat);
            return true;
        }

        /// <summary>
        /// Gets a format by matching against a buffered file.
        /// This is slow, but it is the most likely to succeed.
        /// </summary>
        /// <param name="fileBuffer">A buffered file</param>
        /// <returns>A format handler</returns>
        public virtual IFormat<T> GetFormat(byte[] fileBuffer)
        {
            //Scan each format handler and try the validator
            foreach (IFormat<T> fmt in registeredFormats)
            {
                if (fmt.Parameters.validator(fileBuffer))
                    return fmt;
            }

            return null;
        }

        /// <summary>
        /// Gets a format using an index.
        /// </summary>
        /// <param name="index">The index of the file format</param>
        /// <returns>A format handler</returns>
        public virtual IFormat<T> GetFormat(int index)
        {
            if (index < 0 || index >= registeredFormats.Count)
                return null;

            return registeredFormats[index];
        }

        /// <summary>
        /// Gets a list of formats which have the requested extension.
        /// A List is used because of the potential for conflicting formats using the same file extension.
        /// </summary>
        /// <param name="formatExtension">The requested file extension</param>
        /// <returns>A list of format handlers</returns>
        public virtual List<IFormat<T>> GetFormat(string formatExtension)
        {
            List<IFormat<T>> matches = new();
            foreach (IFormat<T> fmt in registeredFormats)
            {
                if (fmt.Parameters.metadata.extensions.Contains(formatExtension))
                    matches.Add(fmt);
            }

            return matches;
        }
    }
}
