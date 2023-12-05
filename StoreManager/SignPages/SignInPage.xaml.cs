using Oracle.ManagedDataAccess.Client;
using StoreManager.Abstract.Interfaces;
using StoreManager.Admin;
using StoreManager.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private Page clientPage;
        public SignInPage()
        {
            InitializeComponent();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void SignIpBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (GetUserRole(TextBoxAccountName.Text))
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

        private Role? GetUserRole(string userName)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("SELECT GetUserRole(:p_UserName) FROM DUAL", connection))
                    {
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;

                        string userRole = command.ExecuteScalar().ToString();

                        if (!string.IsNullOrEmpty(userRole))
                        {

                            Role roleEnum;
                            if (Enum.TryParse(userRole, true, out roleEnum))
                            {
                                return roleEnum;
                            }
                            else
                            {
                                MessageBox.Show("Помилка при отриманні ролі користувача");
                                return null;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Користувач {userName} не знайдений.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            return null;
        }
    }
    public class SignInButtonIsEnebledConverter : IMultiValueConverter
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
