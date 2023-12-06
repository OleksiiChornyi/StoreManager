using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (Directory.Exists("tmp"))
                    Directory.Delete("tmp", true);
            }
            catch
            {
                MessageBox.Show("I can't delete temporary files from computer");
            }
        }
    }
}
