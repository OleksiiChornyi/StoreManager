using StoreManager.Models.Admin;
using StoreManager.Views.AdminPages;
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
    /// Interaction logic for SignUpAdminPage.xaml
    /// </summary>
    public partial class SignUpAdminPage : Page
    {
        public SignUpAdminPage()
        {
            InitializeComponent();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxAdminPassword.Text.Equals("QWERTY"))
            {
                AdminPage adminPage = new AdminPage(TextBoxAccountName.Text, TextBoxPassword.Text, TextBoxContactInfo.Text);
                if (adminPage.GetAutorization())
                    NavigationService.Navigate(adminPage);
            }
            else
            {
                MessageBox.Show("You entered the wrong administrator password!");
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
