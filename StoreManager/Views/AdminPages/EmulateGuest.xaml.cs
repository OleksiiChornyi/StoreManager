using StoreManager.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreManager.Views.AdminPages
{
    /// <summary>
    /// Interaction logic for EmulateGuest.xaml
    /// </summary>
    public partial class EmulateGuest : Window
    {
        public EmulateGuest()
        {
            InitializeComponent();
            EmulateGuestFrame.Navigate(new ClientPage(isEmulated: true));
        }
    }
}
