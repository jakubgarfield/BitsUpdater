using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitsUpdater
{
    /// <summary>
    /// FileSearchTemplate is a mask used by BitsUpdatePacker to search files that are included in UpdatePackage.
    /// </summary>
    public struct FileSearchTemplate
    {
        /// <summary>
        /// Search pattern used in Directory.GetFiles() method.
        /// </summary>
        public string Pattern
        {
            get;
            set;
        }

        /// <summary>
        /// Root directory used for search.
        /// </summary>
        public DirectoryInfo Directory
        {
            get;
            set;
        }

        /// <summary>
        /// Option to define search behavior.
        /// </summary>
        public SearchOption SearchOption
        {
            get;
            set;
        }

        public FileSearchTemplate(DirectoryInfo directory, string pattern, SearchOption option)
            : this()
        {
            Pattern = pattern;
            Directory = directory;
            SearchOption = option;
        }
    }
}
