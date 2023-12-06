using Microsoft.Win32;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace StoreManager.Views.AdminPages.Interactions
{
    /// <summary>
    /// Interaction logic for Change_Data_Product.xaml
    /// </summary>
    public partial class Change_Data_Product : Window
    {
        readonly AdminStoreInteraction admin;
        readonly ImageItem product;
        string filePath = string.Empty;
        public Change_Data_Product(AdminStoreInteraction admin, ImageItem imageItem)
        {
            InitializeComponent();
            this.admin = admin;
            this.product = imageItem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductNameTextBlock.Text = product.Name;
            DataTable categoriesTable = admin.GetDataFromView("ProductsCategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = row[1],
                    ToolTip = row[0]
                };
                comboBoxCategories.Items.Add(item);
            }
            DataTable descriptionsTable = admin.GetDataFromView("ProductsDescriptionsView");
            foreach (DataRow row in descriptionsTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = row[1],
                    ToolTip = row[0]
                };
                comboBoxDescriptions.Items.Add(item);
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
                        UpdateProductButton.IsEnabled = true;
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
                UpdateProductButton.IsEnabled = true;
            }
        }

        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            string productName = product.Name;
            if (!string.IsNullOrEmpty(ProductNameTextBox.Text))
                productName = ProductNameTextBox.Text;
            int cost = product.price;
            if (!string.IsNullOrEmpty(ProductCostTextBox.Text))
                cost = int.Parse(ProductCostTextBox.Text);
            int categoryId = product.categoryId;
            if (comboBoxCategories.SelectedItem is ComboBoxItem selectedCategory)
                categoryId = int.Parse(selectedCategory.ToolTip.ToString());

            byte[] fileData = product.ImageBytes;
            string fileName = product.fileNameImage;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                fileName = System.IO.Path.GetFileName(filePath);
            }
            int? descriptionId = product.descriptionId;
            if (comboBoxCategories.SelectedItem is ComboBoxItem selectedDescription)
                descriptionId = int.Parse(selectedDescription.ToolTip.ToString());

            if (admin.UpdateProduct(product.productId, productName, cost, categoryId, fileData, fileName, descriptionId))
            {
                MessageBox.Show("The product has been updated");
            }
            this.Close();
        }
    }
}
