﻿using StoreManager.Models.Abstract.Classes;
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

namespace StoreManager.Views.AdminPages.ViewsInfo
{
    /// <summary>
    /// Interaction logic for ViewAuditLogs.xaml
    /// </summary>
    public partial class ViewAuditLogs : Window
    {
        readonly AdminStoreInteraction admin;
        public ViewAuditLogs(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = admin.GetDataFromView("AuditLogsDetails");
            dataGrid.ItemsSource = dataTable.DefaultView;
        }
    }
}
