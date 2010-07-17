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
    public partial class SelectFileControl : UserControl
    {
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(SelectFileControl));
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public SelectFileControl()
        {
            InitializeComponent();
        }

        private void SelectFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                if (!String.IsNullOrEmpty(FilePath))
                {
                    dialog.FileName = FilePath;
                }
                dialog.ShowDialog();
                FilePath = dialog.FileName;
            }
        }
    }
}
