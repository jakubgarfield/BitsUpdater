using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BitsUpdatePacker
{
    internal sealed class UIUpdatePackage
    {
        public Version NextVersion
        {
            get;
            set;
        }

        public string CertificatePath
        {
            get;
            set;
        }

        public bool IsDifferential
        {
            get;
            set;
        }

        public string OutputDirectory
        {
            get;
            set;
        }

        public ObservableCollection<UIFileSearchTemplate> IncludedFiles
        {
            get;
            private set;
        }

        public ObservableCollection<UIFileSearchTemplate> ExcludedFiles
        {
            get;
            private set;
        }

        public UIUpdatePackage()
        {
            IncludedFiles = new ObservableCollection<UIFileSearchTemplate>();
            ExcludedFiles = new ObservableCollection<UIFileSearchTemplate>();
        }
    }
}
