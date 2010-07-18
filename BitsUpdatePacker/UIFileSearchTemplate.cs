using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitsUpdatePacker
{
    internal sealed class UIFileSearchTemplate
    {
        public string Directory
        {
            get;
            set;
        }

        public string Pattern
        {
            get;
            set;
        }

        public SearchOption SearchOption
        {
            get;
            set;
        }

        public UIFileSearchTemplate()
        {
            Pattern = "*";
        }
    }
}
