using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Client.Cart;
using StoreManager.Models.Data;
using StoreManager.Models.Guest;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using StoreManager.Views.Admin.ViewsInfo;
using StoreManager.Views.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace StoreManager.ViewModels.CLient
{
    public class ProductDescriptionViewModel : ViewModelBase
    {

        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }
        private AllUsersInteractions account;
        private StoreCartInteraction myCart = null;
        private readonly ProductDescriptionView productDescriptionView;
        public ProductDescriptionViewModel(StoreCartInteraction myCart, ProductItem productItem)
        {
            this.myCart = myCart;
            account = myCart.client;
            Product = productItem;
            CategoryNameTextBlock = Product.Product.Category.CategoryName;
            ProductNameTextBlock = Product.Product.ProductName;
            ProductPriceTextBlock = Product.Product.Price.ToString();

            if (account.user.UserRole == Role.guest)
            {
                AddOrderItemIsEnabled = false;
                ViewDescriptionIsEnabled = false;
            }
            else
            {
                AddOrderItemIsEnabled = true;
                ViewDescriptionIsEnabled = true;
            }
            if (Product.Product.Description == null)
            {
                ViewDescriptionIsEnabled = false;
                DescriptionButtonTextBlock = "No detailed information available";
            }

            AddOrderItemCommand = new RelayCommand(AddOrderItem);
            ViewDescriptionCommand = new RelayCommand(ViewDescription);

            productDescriptionView = new ProductDescriptionView()
            {
                DataContext = this
            };
            productDescriptionView.Show();
        }
        #region Properties
        public ProductItem _product;
        public ProductItem Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }
        public string _categoryNameTextBlock;
        public string CategoryNameTextBlock
        {
            get => _categoryNameTextBlock;
            set => SetProperty(ref _categoryNameTextBlock, value, nameof(CategoryNameTextBlock));
        }
        public string _categoryDescriptionTextBlock;
        public string CategoryDescriptionTextBlock
        {
            get => _categoryDescriptionTextBlock;
            set => SetProperty(ref _categoryDescriptionTextBlock, value, nameof(CategoryDescriptionTextBlock));
        }
        public string _descriptionButtonTextBlock = "View detailed information";
        public string DescriptionButtonTextBlock
        {
            get => _descriptionButtonTextBlock;
            set => SetProperty(ref _descriptionButtonTextBlock, value, nameof(DescriptionButtonTextBlock));
        }
        public string _productNameTextBlock;
        public string ProductNameTextBlock
        {
            get => _productNameTextBlock;
            set => SetProperty(ref _productNameTextBlock, value, nameof(ProductNameTextBlock));
        }
        public string _productPriceTextBlock;
        public string ProductPriceTextBlock
        {
            get => _productPriceTextBlock;
            set => SetProperty(ref _productPriceTextBlock, value, nameof(ProductPriceTextBlock));
        }
        public bool _addOrderItemIsEnabled;
        public bool AddOrderItemIsEnabled
        {
            get => _addOrderItemIsEnabled;
            set
            {
                _addOrderItemIsEnabled = value;
                OnPropertyChanged(nameof(AddOrderItemIsEnabled));
            }
        }
        public bool _viewDescriptionIsEnabled;
        public bool ViewDescriptionIsEnabled
        {
            get => _viewDescriptionIsEnabled;
            set
            {
                _viewDescriptionIsEnabled = value;
                OnPropertyChanged(nameof(ViewDescriptionIsEnabled));
            }
        }
        #endregion

        public ICommand AddOrderItemCommand { get; }
        public ICommand ViewDescriptionCommand { get; }
        private void AddOrderItem(object parameter)
        {
            myCart.AddOrUpdateItem(Product.Product, 1);
            MessageBox.Show("Product added");
        }
        private void ViewDescription(object parameter)
        {
            if (Product.Product.Description?.DescriptionID == null)
                return;
            try
            {
                if (!Directory.Exists("tmp"))
                {
                    Directory.CreateDirectory("tmp");
                }
                
                string fileName = "tmp\\" + Product.Product.Description.FileName;

                byte[] fileData = Product.Product.Description.FileData;
                account.SaveFileToDisk(fileName, fileData);
                account.OpenFileWithDefaultApplication(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
