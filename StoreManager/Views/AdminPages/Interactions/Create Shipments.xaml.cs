using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace StoreManager.Views.AdminPages.Interactions
{
    /// <summary>
    /// Interaction logic for Create_Shipments.xaml
    /// </summary>
    public partial class Create_Shipments : Window
    {
        readonly AdminStoreInteraction admin;
        public Create_Shipments(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable ordersTable = admin.GetDataFromView("ShipmentsOrdersView");
            foreach (DataRow row in ordersTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = row[1],
                    ToolTip = row[0]
                };
                comboBoxOrders.Items.Add(item);
            }
        }
        private void CreateShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxOrders.SelectedItem is ComboBoxItem selected)
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
                        MessageBox.Show("Shipment created");
                    }
                    else
                    {
                        admin.UpdateShipments(status, (int)shipmentId);
                        MessageBox.Show("Shipment updated");
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("An error occurred");
                }
            }
            else
            {
                MessageBox.Show("An error occurred");
            }
        }
    }
}
