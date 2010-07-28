using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace BitsUpdatePacker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var result = MessageBox.Show(this.MainWindow
                                       , e.Exception.GetType().FullName + "\r\n\r\n" + e.Exception.Message + "\r\n\r\nDo you want to continue?", "Oooops, something went wrong..."
                                       , MessageBoxButton.OK
                                       , MessageBoxImage.Error);
            e.Handled = true;
            App.Current.Shutdown(1);
        }
    }
}
