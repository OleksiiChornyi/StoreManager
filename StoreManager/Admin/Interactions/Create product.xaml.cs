using Microsoft.Win32;
using StoreManager.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
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
    /// Interaction logic for Create_product.xaml
    /// </summary>
    public partial class Create_product : Window
    {
        AdminStoreInteraction admin;
        string filePath = string.Empty;

        public Create_product(AdminStoreInteraction admin)
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
                MessageBox.Show("Необхідно також обрати зображення продукту");
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
                MessageBox.Show("Продукт створено");
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
                        MessageBox.Show("Оберіть файл зображення");
                    }
                }
                else
                {
                    MessageBox.Show("Оберіть один файл");
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
                    MessageBox.Show("Оберіть один файл");
                    return;
                }
                filePath = ofd.FileName;
                CreateProductImageButton.Content = System.IO.Path.GetFileName(filePath);
            }
        }
    }
    public class CreateProductButtonIsEnebledConverter : IMultiValueConverter
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
