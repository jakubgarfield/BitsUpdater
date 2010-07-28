using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using BitsUpdatePacker.Configuration;
using System.Configuration;
using BitsUpdater;

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

        public string PackageUrlDirectory
        {
            get;
            set;
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
            InitializeConfigValues();
        }

        public static IEnumerable<FileSearchTemplate> ConvertFileTemplates(IEnumerable<UIFileSearchTemplate> files)
        {
            var result = new List<FileSearchTemplate>();

            if (files != null)
            {
                foreach (var item in files)
                {
                    if (Directory.Exists(item.Directory))
                    {
                        result.Add(new FileSearchTemplate(new DirectoryInfo(item.Directory), item.Pattern, item.SearchOption));
                    }
                }
            }
            return result;
        }

        private void InitializeConfigValues()
        {
            OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            CertificatePath = ConfigurationManager.AppSettings["CertificatePath"];
            PackageUrlDirectory = ConfigurationManager.AppSettings["PackageDirectoryUrl"];
            AddTemplates(IncludedFiles, (TemplatesConfigSection)ConfigurationManager.GetSection("includedFiles"));
            AddTemplates(ExcludedFiles, (TemplatesConfigSection)ConfigurationManager.GetSection("excludedFiles"));
        }

        private void AddTemplates(ObservableCollection<UIFileSearchTemplate> observableCollection, TemplatesConfigSection configSection)
        {
            foreach (TemplatesElement item in configSection.Templates)
            {
                observableCollection.Add(new UIFileSearchTemplate
                {
                    Directory = item.Directory,
                    Pattern = item.Pattern,
                    SearchOption = item.SearchOption,
                });
            }
        }
    }
}
