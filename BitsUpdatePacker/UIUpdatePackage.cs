using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

namespace BitsUpdatePacker
{
    internal sealed class UIUpdatePackage : DependencyObject
    {
        public static readonly DependencyProperty TokenStringProperty = DependencyProperty.Register("TokenString", typeof(string), typeof(UIUpdatePackage));
        public string TokenString
        {
            get { return (string)GetValue(TokenStringProperty); }
            set { SetValue(TokenStringProperty, value); }
        }

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
