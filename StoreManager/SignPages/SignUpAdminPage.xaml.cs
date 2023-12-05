using StoreManager.Abstract.Interfaces;
using StoreManager.Admin;
using StoreManager.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace StoreManager.SignPages
{
    /// <summary>
    /// Interaction logic for SignUpAdminPage.xaml
    /// </summary>
    public partial class SignUpAdminPage : Page
    {
        private AdminPage adminPage;
        public SignUpAdminPage()
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
            if (TextBoxAdminPassword.Text.Equals("QWERTY"))
            {
                adminPage = new AdminPage(TextBoxAccountName.Text, TextBoxPassword.Text, TextBoxContactInfo.Text);
                if (adminPage.GetAutorization())
                    NavigationService.Navigate(adminPage);
            }
            else
            {
                MessageBox.Show("Ви ввели неправильний пароль адміністратора!");
            }
        }
    }
    public class SignUpAdminButtonIsEnebledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (item is string val && string.IsNullOrEmpty(val.Replace(" ", "")))
                    return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
