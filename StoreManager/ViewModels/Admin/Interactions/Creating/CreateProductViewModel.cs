using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data;
using Microsoft.Win32;
using StoreManager.ViewModels.Help;
using StoreManager.DB_classes;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class CreateProductViewModel : ViewModelBase, IBorderDopViewModel
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly CreateProductView createProductView;

        public CreateProductViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateProductButtonCommand = new RelayCommand(CreateProduct, CanCreateProduct);
            CreateProductImageButtonCommand = new RelayCommand(CreateProductImage);
            BorderDropCommand = new RelayCommand<DragEventArgs>(ExecuteBorderDrop);

            LoadData();

            createProductView = new CreateProductView()
            {
                DataContext = this
            };
            createProductView.Show();
        }

        private void LoadData()
        {
            DropBorderText = "Select or drag a file";
            DataTable categoriesTable = _admin.GetDataFromView("ProductsCategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = row[0];
                item.Content = row[1];
                ComboBoxCategoriesItemsSource.Add(item);
            }
            DataTable descriptionsTable = _admin.GetDataFromView("UpdateDescriptionView");
            foreach (DataRow row in descriptionsTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = row[0];
                item.Content = row[1];
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

        #endregion


        #region Commands

        public ICommand CreateProductButtonCommand { get; }
        public ICommand CreateProductImageButtonCommand { get; }
        public ICommand BorderDropCommand { get; }

        private void CreateProduct(object parameter)
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("You also need to select a product image");
                return;
            }

            var selectedCategory = ComboBoxCategoriesSelectedItem;
            if (selectedCategory != null)
            {
                int categoryId = Convert.ToInt32(selectedCategory.Tag);

                var selectedDescription = ComboBoxDescriptionsSelectedItem;

                Product product;
                if (selectedDescription != null)
                {
                    int descriptionId = Convert.ToInt32(selectedDescription.Tag);
                    product = new Product(ProductNameText, int.Parse(ProductCostText), categoryId, FilePath, descriptionId);
                }
                else
                {
                    product = new Product(ProductNameText, int.Parse(ProductCostText), categoryId, FilePath, null);
                }

                _admin.CreateProduct(product);
                MessageBox.Show("Product created");
                createProductView.Close();
            }
        }


        private bool CanCreateProduct(object parameter)
        {
            return !string.IsNullOrEmpty(ProductNameText) && !string.IsNullOrEmpty(ProductCostText) && ComboBoxCategoriesSelectedItem != null;
        }

        private void CreateProductImage(object parameter)
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
