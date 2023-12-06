using Microsoft.Win32;
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
    /// Interaction logic for Create_Product.xaml
    /// </summary>
    public partial class Create_Product : Window
    {
        readonly AdminStoreInteraction admin;
        string filePath = string.Empty;
        public Create_Product(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable categoriesTable = admin.GetDataFromView("ProductsCategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                comboBoxCategories.Items.Add(item);
            }
            DataTable descriptionsTable = admin.GetDataFromView("ProductsDescriptionsView");
            foreach (DataRow row in descriptionsTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                comboBoxDescriptions.Items.Add(item);
            }
        }

        private void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("You also need to select a product image");
                return;
            }
            var selected = comboBoxCategories.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                int categoryId = int.Parse(selected.ToolTip.ToString());

                selected = comboBoxDescriptions.SelectedItem as ComboBoxItem;
                if (selected != null)
                {
                    int descriptionId = int.Parse(selected.ToolTip.ToString());
                    admin.CreateProduct(ProductNameTextBox.Text, int.Parse(ProductCostTextBox.Text), categoryId, filePath, descriptionId);
                }
                else
                {
                    admin.CreateProduct(ProductNameTextBox.Text, int.Parse(ProductCostTextBox.Text), categoryId, filePath, null);
                }
                MessageBox.Show("Product created");
                this.Close();
            }
        }

        private void ProductCostTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void CreateProductImageButton_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void CreateProductImageButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string filePath = files[0];
                    string fileExtension = System.IO.Path.GetExtension(filePath);
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp")
                    {
                        this.filePath = filePath;
                        CreateProductImageButton.Content = System.IO.Path.GetFileName(filePath);
                    }
                    else
                    {
                        MessageBox.Show("Select an image file");
                    }
                }
                else
                {
                    MessageBox.Show("Select one file");
                }
            }
        }

        private void CreateProductImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.png;*.gif;*.bmp",
                InitialDirectory = "c:\\"
            };
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileNames.Length > 1 || ofd.FileNames.Length < 1)
                {
                    MessageBox.Show("Select one file");
                    return;
                }
                filePath = ofd.FileName;
                CreateProductImageButton.Content = System.IO.Path.GetFileName(filePath);
            }
        }
    }
}
