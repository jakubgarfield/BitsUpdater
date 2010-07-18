using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BitsUpdater;
using System.Collections.ObjectModel;
using System.IO;
using System.Configuration;
using BitsUpdatePacker.Configuration;

namespace BitsUpdatePacker
{
    public partial class MainWindow : Window
    {
        private UIUpdatePackage _package = new UIUpdatePackage();

        public MainWindow()
        {
            InitializeComponent();
            InitializeConfigValues();
            this.DataContext = _package;            
        }

        private void InitializeConfigValues()
        {
            _package.OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            _package.CertificatePath = ConfigurationManager.AppSettings["CertificatePath"];
            AddTemplates(_package.IncludedFiles, (TemplatesConfigSection)ConfigurationManager.GetSection("includedFiles"));
            AddTemplates(_package.ExcludedFiles, (TemplatesConfigSection)ConfigurationManager.GetSection("excludedFiles"));
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

        private void RemoveTemplateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var template = e.Parameter as UIFileSearchTemplate;
            if (template != null)
            {
                ((ObservableCollection<UIFileSearchTemplate>)((ItemsControl)e.Source).ItemsSource).Remove(template);
            }
        }

        private void AddNewTemplateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "Include")
            {
                _package.IncludedFiles.Add(new UIFileSearchTemplate());                
            }
            else if (e.Parameter.ToString() == "Exclude")
            {
                _package.ExcludedFiles.Add(new UIFileSearchTemplate());
            }
        }

        private void CreatePackageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsValid((Grid)sender))
            {
                var package = new UpdatePackage(_package.CertificatePath, _package.NextVersion, _package.IsDifferential)
                {
                    IncludedFiles = ConvertFileTemplates(_package.IncludedFiles),
                    ExcludedFiles = ConvertFileTemplates(_package.ExcludedFiles),
                };
                package.Create(_package.OutputDirectory);
                MessageBox.Show("Update package was successfully completed.", "Package Completed!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private IEnumerable<FileSearchTemplate> ConvertFileTemplates(IEnumerable<UIFileSearchTemplate> files)
        {
            var result = new List<FileSearchTemplate>();

            if (files != null)
            {
                foreach (var item in files)
                {
                    if (Directory.Exists(item.Directory))
                    {
                        result.Add(new FileSearchTemplate(item.Directory, item.Pattern, item.SearchOption));
                    }
                }
            }
            return result;
        }

        private bool IsValid(Grid grid)
        {
            BindingExpression version = txtVersion.GetBindingExpression(TextBox.TextProperty);
            version.UpdateSource();

            BindingExpression certificatePath = sfcCertificate.GetBindingExpression(SelectFileControl.FilePathProperty);
            certificatePath.UpdateSource();

            BindingExpression outputPath = sfcOutput.GetBindingExpression(SelectFolderControl.FolderPathProperty);
            outputPath.UpdateSource();

            return !version.HasError && !certificatePath.HasError && !outputPath.HasError;
        }

    }
}
