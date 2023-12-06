﻿using StoreManager.Models.Abstract.Classes;
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
    /// Interaction logic for Create_Inventory.xaml
    /// </summary>
    public partial class Create_Inventory : Window
    {
        readonly AdminStoreInteraction admin;
        public Create_Inventory(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable productTable = admin.GetDataFromView("InventoryProductsView");
            DataTable warehousesTable = admin.GetDataFromView("InventoryWarehousesView");
            foreach (DataRow row in productTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                comboBoxProductName.Items.Add(item);
            }
            foreach (DataRow row in warehousesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                comboBoxWarehousesName.Items.Add(item);
            }
        }

        private void CreateInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = comboBoxProductName.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                int productId = int.Parse(selected.ToolTip.ToString());

                selected = comboBoxWarehousesName.SelectedItem as ComboBoxItem;
                if (selected != null)
                {
                    int warehouseId = int.Parse(selected.ToolTip.ToString());
                    admin.CreateInventory(productId, int.Parse(QuantityOnHandTextBox.Text), warehouseId);
                }
                MessageBox.Show("Inventory added");
                this.Close();
            }
        }

        private void QuantityOnHandTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}