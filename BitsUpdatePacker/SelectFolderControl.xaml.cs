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

namespace BitsUpdatePacker
{
    public partial class SelectFolderControl : UserControl
    {
        public static readonly DependencyProperty FolderPathProperty = DependencyProperty.Register("FolderPath", typeof(string), typeof(SelectFolderControl));
        public string FolderPath
        {
            get { return (string)GetValue(FolderPathProperty); }
            set { SetValue(FolderPathProperty, value); }
        }

        public SelectFolderControl()
        {
            InitializeComponent();
        }

        private void SelectFolderExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!String.IsNullOrEmpty(FolderPath))
                {
                    dialog.SelectedPath = FolderPath;
                }
                dialog.ShowDialog();
                FolderPath = dialog.SelectedPath;
            }
        }
    }
}
