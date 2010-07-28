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
using System.Xml.Serialization;

namespace BitsUpdatePacker
{
    public partial class MainWindow : Window
    {
        private UIUpdatePackage _package = new UIUpdatePackage();
        private static readonly XmlSerializer _manifestSerializer = new XmlSerializer(typeof(XmlUpdateManifest));

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _package;
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
                var package = new UpdatePackage(_package.CertificatePath, _package.NextVersion, _package.IsDifferential);
                foreach (var item in UIUpdatePackage.ConvertFileTemplates(_package.IncludedFiles))
                {
                    package.IncludedFiles.Add(item);
                }
                foreach (var item in UIUpdatePackage.ConvertFileTemplates(_package.ExcludedFiles))
                {
                    package.ExcludedFiles.Add(item);
                }
                _package.TokenString = package.Create(new DirectoryInfo(_package.OutputDirectory));
                CreateManifest();
                MessageBox.Show(this
                              , "Update package was successfully completed."
                              , "Package Completed!"
                              , MessageBoxButton.OK
                              , MessageBoxImage.Asterisk);
            }
        }

        private void CreateManifest()
        {
            XmlUpdateManifest manifest = new XmlUpdateManifest
            {
                Url = ((_package.PackageUrlDirectory.EndsWith("/")) ? _package.PackageUrlDirectory : _package.PackageUrlDirectory + "/") + string.Format(UpdatePackage.AssemblyName, _package.NextVersion) + UpdatePackage.AssemblySuffix + UpdatePackage.PackageSuffix,
                Version = _package.NextVersion.ToString(),
            };

            using (var manifestFile = new FileStream(System.IO.Path.Combine(_package.OutputDirectory, "UpdateManifest.xml"), FileMode.Create))
            {
                _manifestSerializer.Serialize(manifestFile, manifest);
            }
        }

        private bool IsValid(Grid grid)
        {
            BindingExpression version = txtVersion.GetBindingExpression(TextBox.TextProperty);
            version.UpdateSource();

            BindingExpression certificatePath = sfcCertificate.GetBindingExpression(SelectFileControl.FilePathProperty);
            certificatePath.UpdateSource();

            BindingExpression outputPath = sfcOutput.GetBindingExpression(SelectFolderControl.FolderPathProperty);
            outputPath.UpdateSource();

            BindingExpression url = txtUrl.GetBindingExpression(TextBox.TextProperty);
            url.UpdateSource();
            return !version.HasError && !certificatePath.HasError && !outputPath.HasError && !url.HasError;
        }

    }
}
