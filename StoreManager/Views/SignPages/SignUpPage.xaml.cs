using StoreManager.Models.Abstract.Interfaces;
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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientPage clientPage = new ClientPage(TextBoxAccountName.Text, TextBoxPassword.Text, TextBoxContactInfo.Text, Role.client);
            if (clientPage.GetAutorization())
                NavigationService.Navigate(clientPage);
        }
    }
}
