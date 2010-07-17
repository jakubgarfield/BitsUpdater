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
                _package.FilesForUpdate.Add(new UIFileSearchTemplate());                
            }
            else if (e.Parameter.ToString() == "Exclude")
            {
                _package.FilesNotForUpdate.Add(new UIFileSearchTemplate());
            }
        }

        private void CreatePackageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsValid((Grid)sender))
            {
                var package = new UpdatePackage(_package.CertificatePath, _package.NextVersion, _package.IsDifferential)
                {
                    FilesForUpdate = ConvertFileTemplates(_package.FilesForUpdate),
                    FilesNotForUpdate = ConvertFileTemplates(_package.FilesNotForUpdate),
                };
                package.Create(_package.OutputDirectory);
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
