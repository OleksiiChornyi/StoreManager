using StoreManager.Models.Abstract.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreManager.Views.SignPages
{
    /// <summary>
    /// Interaction logic for MainSignPage.xaml
    /// </summary>
    public partial class MainSignPage : Page
    {
        public MainSignPage()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignInPage());
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                NavigationService.Navigate(new SignUpAdminPage());
            }
            else
            {
                NavigationService.Navigate(new SignUpPage());
            }
        }

        private void ContGuestButton_Click(object sender, RoutedEventArgs e)
        {
            ClientStoreInteraction.CreateGuestIfNotExist();
            var clientPage = new ClientPage();
            if (clientPage.GetAutorization())
                NavigationService.Navigate(clientPage);
        }
    }
}
