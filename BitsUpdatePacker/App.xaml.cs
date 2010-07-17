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
            MessageBox.Show(e.Exception.GetType().FullName + "\r\n\r\n" + e.Exception.Message, "Oooops, something went wrong...");
            e.Handled = true;
            App.Current.Shutdown(1);
        }
    }
}
