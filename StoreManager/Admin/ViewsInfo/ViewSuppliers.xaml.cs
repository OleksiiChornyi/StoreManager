﻿using StoreManager.Abstract.Classes;
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
using System.Windows.Shapes;

namespace StoreManager.Admin.ViewsInfo
{
    /// <summary>
    /// Interaction logic for ViewSuppliers.xaml
    /// </summary>
    public partial class ViewSuppliers : Window
    {
        readonly AdminStoreInteraction admin;
        public ViewSuppliers(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = admin.GetDataFromView("SupplierDetails");
            dataGrid.ItemsSource = dataTable.DefaultView;
        }
    }
}
