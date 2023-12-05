using StoreManager.Abstract.Classes;
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
using System.Windows.Shapes;

namespace StoreManager.Admin.Interactions
{
    /// <summary>
    /// Interaction logic for Create_Category.xaml
    /// </summary>
    public partial class Create_Category : Window
    {
        AdminStoreInteraction admin;
        public Create_Category(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void CreateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            admin.CreateCategory(CategoryNameTextBox.Text, CategoryDescriptionTextBox.Text);
            MessageBox.Show("Категорія створена");
            this.Close();
        }
    }
    public class CreateCategoryButtonIsEnebledConverter : IMultiValueConverter
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
