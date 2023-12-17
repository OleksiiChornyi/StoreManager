using Microsoft.Win32;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Data;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class ChangeDataProductViewModel : ViewModelBase, IInitializable
    {
        private INavigationService _navigation;
        private AdminStoreInteraction _admin;
        private ProductItem _productItem;
        public ProductItem ProductItem
        {
            get => _productItem;
            set
            {
                _productItem = value;
                OnPropertyChanged();
            }
        }
        private string _filePath = string.Empty;

        public ChangeDataProductViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            CreateProductImageButtonCommand = new RelayCommand(CreateProductImageButton);
            CreateProductImageButtonDragEnterCommand = new RelayCommand(CreateProductImageButtonDragEnter);
            CreateProductImageButtonDropCommand = new RelayCommand(CreateProductImageButtonDrop);
            UpdateProductButtonCommand = new RelayCommand(UpdateProductButton);
            ProductCostTextBoxPreviewTextInputCommand = new RelayCommand(ProductCostTextBoxPreviewTextInput);
        }

        public RelayCommand CreateProductImageButtonCommand { get; }
        public RelayCommand CreateProductImageButtonDragEnterCommand { get; }
        public RelayCommand CreateProductImageButtonDropCommand { get; }
        public RelayCommand UpdateProductButtonCommand { get; }
        public RelayCommand ProductCostTextBoxPreviewTextInputCommand { get; }

        public void Initialize(object parameter)
        {
            if (parameter is Tuple<AdminStoreInteraction, ProductItem> tuple)
            {
                _admin = tuple.Item1;
                ProductItem = tuple.Item2;

                ProductNameTextBlock = ProductItem.Product.ProductName;
                DataTable categoriesTable = _admin.GetDataFromView("ProductsCategoriesView");
                foreach (DataRow row in categoriesTable.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = row[1],
                        ToolTip = row[0]
                    };
                    ComboBoxCategoriesItems.Add(item);
                }
                DataTable descriptionsTable = _admin.GetDataFromView("ProductsDescriptionsView");
                foreach (DataRow row in descriptionsTable.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = row[1],
                        ToolTip = row[0]
                    };
                    ComboBoxDescriptionsItems.Add(item);
                }
            }
            else
            {
                throw new ArgumentException("Invalid parameter type");
            }
        }
        #region Properties
        private string _productNameTextBlock;
        public string ProductNameTextBlock
        {
            get => _productNameTextBlock;
            set => SetProperty(ref _productNameTextBlock, value, nameof(ProductNameTextBlock));
        }
        private string _productNameText;
        public string ProductNameText
        {
            get => _productNameText;
            set => SetProperty(ref _productNameText, value, nameof(ProductNameText));
        }

        private string _productCostText;
        public string ProductCostText
        {
            get => _productCostText;
            set => SetProperty(ref _productCostText, value, nameof(ProductCostText));
        }

        private string _createProductImageButtonContent;
        public string CreateProductImageButtonContent
        {
            get => _createProductImageButtonContent;
            set => SetProperty(ref _createProductImageButtonContent, value, nameof(CreateProductImageButtonContent));
        }

        private ObservableCollection<ComboBoxItem> _comboBoxCategoriesItems;
        public ObservableCollection<ComboBoxItem> ComboBoxCategoriesItems
        {
            get => _comboBoxCategoriesItems;
            set
            {
                _comboBoxCategoriesItems = value;
                OnPropertyChanged(nameof(ComboBoxCategoriesItems));
            }
        }

        private ComboBoxItem _comboBoxCategoriesSelectedItem;
        public ComboBoxItem ComboBoxCategoriesSelectedItem
        {
            get => _comboBoxCategoriesSelectedItem;
            set
            {
                _comboBoxCategoriesSelectedItem = value;
                OnPropertyChanged(nameof(ComboBoxCategoriesSelectedItem));
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxDescriptionsItems;
        public ObservableCollection<ComboBoxItem> ComboBoxDescriptionsItems
        {
            get => _comboBoxDescriptionsItems;
            set
            {
                _comboBoxDescriptionsItems = value;
                OnPropertyChanged(nameof(ComboBoxDescriptionsItems));
            }
        }

        private ComboBoxItem _comboBoxDescriptionsSelectedItem;
        public ComboBoxItem ComboBoxDescriptionsSelectedItem
        {
            get => _comboBoxDescriptionsSelectedItem;
            set
            {
                _comboBoxDescriptionsSelectedItem = value;
                OnPropertyChanged(nameof(ComboBoxDescriptionsSelectedItem));
            }
        }
        #endregion

        private void CreateProductImageButton(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Image files (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.png;*.gif;*.bmp",
                InitialDirectory = "c:\\"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                CreateProductImageButtonContent = System.IO.Path.GetFileName(selectedFilePath);
                UpdateProductButtonCommand.RaiseCanExecuteChanged();
            }
        }


        private void CreateProductImageButtonDragEnter(object parameter)
        {
            DragEventArgs dragEventArgs = parameter as DragEventArgs;

            if (dragEventArgs?.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                dragEventArgs.Effects = DragDropEffects.Copy;
            }
        }

        private void CreateProductImageButtonDrop(object parameter)
        {
            if (parameter is DragEventArgs e && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string filePath = files[0];
                    string fileExtension = System.IO.Path.GetExtension(filePath);
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp")
                    {
                        _filePath = filePath;
                        CreateProductImageButtonContent = System.IO.Path.GetFileName(filePath);
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

        private void UpdateProductButton(object parameter)
        {
            int productId = ProductItem.Product.ProductID;
            string productName = string.IsNullOrEmpty(ProductNameText) ? ProductItem.Product.ProductName: ProductNameText;
            int cost = ProductItem.Product.Price;

            if (!string.IsNullOrEmpty(ProductCostText))
            {
                if (int.TryParse(ProductCostText, out int parsedCost))
                {
                    cost = parsedCost;
                }
                else
                {
                    MessageBox.Show("Invalid cost value. Please enter a valid number.");
                    return;
                }
            }

            int categoryId = ProductItem.Product.Category.CategoryID;
            if (ComboBoxCategoriesSelectedItem != null)
            {
                if (int.TryParse(ComboBoxCategoriesSelectedItem.ToolTip.ToString(), out int parsedCategoryId))
                {
                    categoryId = parsedCategoryId;
                }
            }

            byte[] fileData = ProductItem.Product.BinaryContent.Content;
            string fileName = ProductItem.Product.BinaryContent.FileName;

            if (!string.IsNullOrEmpty(_filePath))
            {
                fileData = File.ReadAllBytes(_filePath);
                fileName = System.IO.Path.GetFileName(_filePath);
            }

            int? descriptionId = ProductItem.Product.Description.DescriptionID;
            if (ComboBoxDescriptionsSelectedItem != null)
            {
                if (int.TryParse(ComboBoxDescriptionsSelectedItem.ToolTip.ToString(), out int parsedDescriptionId))
                {
                    descriptionId = parsedDescriptionId;
                }
            }

            /*if (_admin.UpdateProduct(productId, productName, cost, categoryId, fileData, fileName, descriptionId))
            {
                MessageBox.Show("The product has been updated");
                _navigation.GoBackWhile();
            }*/
        }

        private void ProductCostTextBoxPreviewTextInput(object parameter)
        {
            if (parameter is TextCompositionEventArgs e)
            {
                if (!char.IsDigit(e.Text, 0))
                {
                    e.Handled = true;
                }
            }
        }

    }
}
