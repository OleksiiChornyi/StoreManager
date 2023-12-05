using StoreManager.Abstract.Classes;
using StoreManager.Abstract.Interfaces;
using StoreManager.Client;
using StoreManager.Guest;
using StoreManager.SignPages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StoreManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new StoreForGuest("Guest", "1111", "Guest", Role.guest);
            MainFrame.Navigate(new MainSignPage());
        }

        private void StoreManagerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (Directory.Exists("tmp"))
                    Directory.Delete("tmp", true);
            }
            catch
            {
                MessageBox.Show("Я не можу видалити тимчасові файли з пк");
            }
        }
    }
}
