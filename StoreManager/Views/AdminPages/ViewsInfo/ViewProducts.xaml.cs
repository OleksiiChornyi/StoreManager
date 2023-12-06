using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ViewProducts.xaml
    /// </summary>
    public partial class ViewProducts : Window
    {
        readonly AdminStoreInteraction admin;
        public ViewProducts(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = admin.GetDataFromView("ProductDetailsView");
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (dataGrid != null && dataGrid.SelectedItems != null && dataGrid.SelectedItems.Count == 1)
            {
                var selectedItem = dataGrid.SelectedItem;
                if (selectedItem == null)
                    return;
                int productID = int.Parse(((System.Data.DataRowView)selectedItem).Row.ItemArray[0].ToString());
                if (productID < 0)
                    return;
                try
                {
                    if (!Directory.Exists("tmp"))
                    {
                        Directory.CreateDirectory("tmp");
                    }

                    (string fileName, byte[] fileData) = admin.GetProductImage(productID);

                    if (fileData == null)
                    {
                        return;
                    }

                    fileName = "tmp\\" + fileName;

                    admin.SaveFileToDisk(fileName, fileData);
                    admin.OpenFileWithDefaultApplication(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
