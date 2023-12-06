using Oracle.ManagedDataAccess.Client;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Admin;
using StoreManager.Client;
using StoreManager.Views.AdminPages;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void SignIpBtn_Click(object sender, RoutedEventArgs e)
        {
            Page clientPage;
            switch (ClientStoreInteraction.GetUserRole(TextBoxAccountName.Text))
            {
                case Role.client:
                    clientPage = new ClientPage(TextBoxAccountName.Text, TextBoxPassword.Text, "", Role.client);
                    if (((ClientPage)clientPage).GetAutorization())
                        NavigationService.Navigate(clientPage);
                    break;
                case Role.guest:
                    clientPage = new ClientPage();
                    if (((ClientPage)clientPage).GetAutorization())
                        NavigationService.Navigate(clientPage);
                    break;
                case Role.admin:
                    clientPage = new AdminPage(TextBoxAccountName.Text, TextBoxPassword.Text);
                    if (((AdminPage)clientPage).GetAutorization())
                        NavigationService.Navigate(clientPage);
                    break;
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
