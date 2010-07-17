using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BitsUpdatePacker
{
    public sealed class UIUpdatePackage
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

        public ObservableCollection<UIFileSearchTemplate> FilesForUpdate
        {
            get;
            private set;
        }

        public ObservableCollection<UIFileSearchTemplate> FilesNotForUpdate
        {
            get;
            private set;
        }

        public UIUpdatePackage()
        {
            FilesForUpdate = new ObservableCollection<UIFileSearchTemplate>();
            FilesNotForUpdate = new ObservableCollection<UIFileSearchTemplate>();
        }
    }
}
