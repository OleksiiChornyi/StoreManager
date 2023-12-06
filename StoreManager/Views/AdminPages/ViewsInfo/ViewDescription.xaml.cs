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
    /// Interaction logic for ViewDescription.xaml
    /// </summary>
    public partial class ViewDescription : Window
    {
        readonly AdminStoreInteraction admin;
        public ViewDescription(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = admin.GetDataFromView("DescriptionsView");
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
                int descriptionID = int.Parse(((System.Data.DataRowView)selectedItem).Row.ItemArray[0].ToString());
                if (descriptionID < 0)
                    return;
                try
                {
                    if (!Directory.Exists("tmp"))
                    {
                        Directory.CreateDirectory("tmp");
                    }

                    (string fileName, byte[] fileData) = admin.GetDescriptionData(descriptionID);

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
