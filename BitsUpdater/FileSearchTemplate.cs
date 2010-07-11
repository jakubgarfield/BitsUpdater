using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitsUpdater
{
    public sealed class FileSearchTemplate
    {
        public string Pattern
        {
            get;
            set;
        }

        public string Directory
        {
            get;
            set;
        }

        public SearchOption SearchOption
        {
            get;
            set;
        }

        public FileSearchTemplate(string directory, string pattern, SearchOption option)
        {
            Pattern = pattern;
            Directory = directory;
            SearchOption = option;
        }
    }
}
