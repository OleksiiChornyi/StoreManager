using StoreManager.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Create_Shipments.xaml
    /// </summary>
    public partial class Create_Shipments : Window
    {
        AdminStoreInteraction admin;
        public Create_Shipments(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable ordersTable = admin.GetDataFromView("ShipmentsOrdersView ");
            foreach (DataRow row in ordersTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                comboBoxOrders.Items.Add(item);
            }
        }

        private void CreateShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = comboBoxOrders.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                int OrderId = int.Parse(selected.ToolTip.ToString());

                selected = comboBoxStatus.SelectedItem as ComboBoxItem;
                if (selected != null)
                {
                    string status = selected.Content.ToString();
                    int? shipmentId = admin.HasShipment(OrderId);
                    if (shipmentId == null)
                    {
                        admin.CreateShipments(status, OrderId);
                        MessageBox.Show("Відвантаження створено");
                    }
                    else
                    {
                        admin.UpdateShipments(status, (int)shipmentId);
                        MessageBox.Show("Відвантаження оновлено");
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Виникла помилка");
                }
            }
            else
            {
                MessageBox.Show("Виникла помилка");
            }
        }
    }
    public class CreateShipmentsButtonIsEnebledConverter : IMultiValueConverter
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
