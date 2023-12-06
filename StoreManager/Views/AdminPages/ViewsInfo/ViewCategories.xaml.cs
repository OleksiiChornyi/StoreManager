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
    /// Interaction logic for ViewCategories.xaml
    /// </summary>
    public partial class ViewCategories : Window
    {
        readonly AdminStoreInteraction admin;
        public ViewCategories(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = admin.GetCategoryHierarchy();
            //var dataTable = admin.GetDataFromView("CategoriesView");
            dataGrid.ItemsSource = dataTable.DefaultView;
        }
    }
}