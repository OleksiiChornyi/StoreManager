using Microsoft.Win32;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Help;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using StoreManager.Views.Admin.Interactions.Updating;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateProductViewModel : ViewModelBase, IBorderDopViewModel
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateProductsView updateProductView;

        public UpdateProductViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateProductButtonCommand = new RelayCommand(UpdateProduct);
            CreateProductImageButtonCommand = new RelayCommand(UpdateProductImage);
            BorderDropCommand = new RelayCommand<DragEventArgs>(ExecuteBorderDrop);
            LoadData();

            updateProductView = new UpdateProductsView()
            {
                DataContext = this
            };
            updateProductView.Show();
        }

        private void LoadData()
        {
            DropBorderText = "Select or drag a file";
            DataTable productTable = _admin.GetDataFromView("UpdateProductsView");
            List<Product> products = new List<Product>();
            foreach (DataRow row in productTable.Rows)
            {
                Product product;
                Description description = null;
                BinaryContent binaryContent = null;
                int descriptionID = 0;
                if (int.TryParse(row[2].ToString(), out descriptionID))
                    description = new Description() { DescriptionID = descriptionID, FileName = row[3].ToString() };
                int contentID = 0;
                if (int.TryParse(row[4].ToString(), out contentID))
                    binaryContent = new BinaryContent() { ContentID = contentID, FileName = row[5].ToString() };
                product = new Product(int.Parse(row[0].ToString()), row[1].ToString(), description, binaryContent, int.Parse(row[6].ToString()), 
                    new Category() { CategoryID = int.Parse(row[7].ToString()), CategoryName = row[8].ToString() }, int.Parse(row[9].ToString()));
                products.Add(product);
            }
            foreach(var product in products)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = product;
                item.Content = product.ProductName;
                ComboBoxUpdateItemSource.Add(item);
            }
            DataTable categoriesTable = _admin.GetDataFromView("ProductsCategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                Category category = new Category() { CategoryID = int.Parse(row[0].ToString()), CategoryName = row[1].ToString() };
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = category;
                item.Content = category.CategoryName;
                ComboBoxCategoriesItemsSource.Add(item);
            }
            DataTable descriptionsTable = _admin.GetDataFromView("UpdateDescriptionView");
            foreach (DataRow row in descriptionsTable.Rows)
            {
                Description description = new Description() { DescriptionID = int.Parse(row[0].ToString()), FileName = row[1].ToString() };
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = description;
                item.Content = description.FileName;
                ComboBoxDescriptionsItemsSource.Add(item);
            }
        }

        #region Properties
        public string FilePath { get; set; }

        private string _productNameText;
        public string ProductNameText
        {
            get => _productNameText;
            set => SetProperty(ref _productNameText, value, nameof(ProductNameText));
        }

        private string _dropBorderText;
        public string DropBorderText
        {
            get => _dropBorderText;
            set => SetProperty(ref _dropBorderText, value, nameof(DropBorderText));
        }
        private string _productCostText;
        public string ProductCostText
        {
            get => _productCostText;
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _productCostText, value, nameof(ProductCostText));
                }
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxCategoriesItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCategoriesItemsSource
        {
            get => _comboBoxCategoriesItemsSource;
            set
            {
                if (_comboBoxCategoriesItemsSource != value)
                {
                    _comboBoxCategoriesItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCategoriesItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxCategoriesSelectedItem;
        public ComboBoxItem ComboBoxCategoriesSelectedItem
        {
            get => _comboBoxCategoriesSelectedItem;
            set
            {
                if (_comboBoxCategoriesSelectedItem != value)
                {
                    _comboBoxCategoriesSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxCategoriesSelectedItem));
                }
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxDescriptionsItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxDescriptionsItemsSource
        {
            get => _comboBoxDescriptionsItemsSource;
            set
            {
                if (_comboBoxDescriptionsItemsSource != value)
                {
                    _comboBoxDescriptionsItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxDescriptionsItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxDescriptionsSelectedItem;
        public ComboBoxItem ComboBoxDescriptionsSelectedItem
        {
            get => _comboBoxDescriptionsSelectedItem;
            set
            {
                if (_comboBoxDescriptionsSelectedItem != value)
                {
                    _comboBoxDescriptionsSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxDescriptionsSelectedItem));
                }
            }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxUpdateItemSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxUpdateItemSource
        {
            get => _comboBoxUpdateItemSource;
            set
            {
                if (_comboBoxUpdateItemSource != value)
                {
                    _comboBoxUpdateItemSource = value;
                    OnPropertyChanged(nameof(ComboBoxUpdateItemSource));
                }
            }
        }

        private ComboBoxItem _comboBoxUpdateSelectedItem;
        public ComboBoxItem ComboBoxUpdateSelectedItem
        {
            get => _comboBoxUpdateSelectedItem;
            set
            {
                if (_comboBoxUpdateSelectedItem != value)
                {
                    Product product = value.Tag as Product;
                    ProductNameText = product.ProductName;
                    ProductCostText = product.Price.ToString();
                    DropBorderText = product.BinaryContent.FileName;

                    ComboBoxItem selectItem = ComboBoxCategoriesItemsSource
                        .FirstOrDefault(item => (item.Tag as Category)?.CategoryID == product?.Category?.CategoryID);
                    ComboBoxCategoriesSelectedItem = selectItem;

                    selectItem = ComboBoxDescriptionsItemsSource
                        .FirstOrDefault(item => (item.Tag as Description)?.DescriptionID == product?.Description?.DescriptionID);
                    ComboBoxDescriptionsSelectedItem = selectItem;

                    _comboBoxUpdateSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxUpdateSelectedItem));
                }
            }
        }

        #endregion


        #region Commands

        public ICommand UpdateProductButtonCommand { get; }
        public ICommand CreateProductImageButtonCommand { get; }
        public ICommand BorderDropCommand { get; }

        private void UpdateProduct(object parameter)
        {
            if (ComboBoxUpdateSelectedItem == null)
            {
                MessageBox.Show("You need select a product to update");
                return;
            }

            Product product = ComboBoxUpdateSelectedItem.Tag as Product;

            product.ProductName = ProductNameText;
            product.Price = int.Parse(ProductCostText);

            product.BinaryContent = FilePath == null || product.BinaryContent.FileName == FilePath ? product.BinaryContent : _admin.CreateBinaryContent(new BinaryContent(FilePath));

            product.Category = ComboBoxCategoriesSelectedItem.Tag as Category;
            product.Description = ComboBoxDescriptionsSelectedItem.Tag as Description;

            if (_admin.UpdateProduct(product))
            {
                MessageBox.Show("Product updated");
                updateProductView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update this product");
            }
        }

        private void UpdateProductImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Image files (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.png;*.gif;*.bmp",
                InitialDirectory = "c:\\"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length != 1)
                {
                    MessageBox.Show("Select one file");
                    return;
                }

                FilePath = openFileDialog.FileName;
                DropBorderText = System.IO.Path.GetFileName(FilePath);
            }
        }

        public void ExecuteBorderDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string filePath = files[0];
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileExtension = System.IO.Path.GetExtension(filePath);
                        if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp")
                        {
                            FilePath = filePath;
                            DropBorderText = System.IO.Path.GetFileName(filePath);
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
                else
                {
                    MessageBox.Show("Select one file");
                }
            }
        }
        #endregion

        #region Helpers
        private bool IsTextAllowed(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }
        #endregion
    }
}
