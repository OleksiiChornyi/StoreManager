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
    /// Interaction logic for Create_Warehouses.xaml
    /// </summary>
    public partial class Create_Warehouses : Window
    {
        readonly AdminStoreInteraction admin;
        public Create_Warehouses(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void CreateSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            admin.CreateWarehouses(WarehousesNameTextBox.Text, WarehousesLocationTextBox.Text, int.Parse(WarehousesCapacityTextBox.Text), int.Parse(WarehousesAvailabilityTextBox.Text));
            MessageBox.Show("Склад додано");
            this.Close();
        }

        private void WarehousesAvailabilityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void WarehousesCapacityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
    public class CreateWarehousesButtonIsEnebledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (item is string val && string.IsNullOrEmpty(val.Replace(" ", "")))
                    return false;
                if (item == null)
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
