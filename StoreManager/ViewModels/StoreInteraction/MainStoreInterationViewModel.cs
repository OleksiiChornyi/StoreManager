using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Updating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using StoreManager.Models.Abstract.Interfaces;
using System.Security.Principal;
using StoreManager.ViewModels.Services;
using StoreManager.Models.Client;
using StoreManager.Models.Guest;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Sign;
using StoreManager.DB_classes;
using StoreManager.Models.Data;

namespace StoreManager.ViewModels.StoreInteraction
{
    internal class MainStoreInterationViewModel : ViewModelBase
    {
        private enum sortStyle { up, down, cencel }
        private readonly AllUsersInteractions account;
        private readonly Window ProductsView;
        private List<Category> categoryItems = new List<Category>();
        private List<ProductItem> ProductItems = new List<ProductItem>();
        private List<ProductItem> myChangeProductItems = new List<ProductItem>();

        public MainStoreInterationViewModel(INavigationService navigation = null, AllUsersInteractions account = null)
        {
            if (account == null)
            {
                Checkings.CreateGuestIfNotExist();
                this.account = new StoreForGuest();
            }
            else
            {
                this.account = account;
            }

            Navigation = navigation;

            ButtonProfileCommand = new RelayCommand(ButtonProfile);
            ButtonSortPriceCommand = new RelayCommand(ButtonSortPrice);
            ButtonSortPopularityCommand = new RelayCommand(ButtonSortPopularity);
            ButtonUpdateCommand = new RelayCommand(ButtonUpdate);
            MenuItemCommand = new RelayCommand(ExecuteMenuItemCommand, CanExecuteMenuItem);
            MouseDoubleClickCommand = new RelayCommand(MouseDoubleClick);

            LoadData();

            if (account?.user?.UserRole== Role.admin)
            {
                ProductsView = new UpdateProductsView()
                {
                    DataContext = this
                };
                ProductsView.Show();
            }
        }

        #region Functions
        private async void LoadData()
        {

            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    ButtonCartIsEnabled = true;
                    ButtonExitIsEnabled = true;
                    MenuItemIsEnabled = true;
                    ButtonProfileContent = "Profile";
                    MenuItemHeader = "Delete the item";
                    break;
                case Role.client:
                    ButtonCartIsEnabled = true;
                    ButtonExitIsEnabled = true;
                    MenuItemIsEnabled = true;
                    ButtonProfileContent = "Profile";
                    MenuItemHeader = "Add to cart";
                    break;
                case Role.guest:
                    ButtonCartIsEnabled = false;
                    ButtonExitIsEnabled = false;
                    MenuItemIsEnabled = false;
                    ButtonProfileContent = "Autorize";
                    MenuItemHeader = string.Empty;
                    break;
                default:
                    ButtonCartIsEnabled = false;
                    ButtonExitIsEnabled = false;
                    MenuItemIsEnabled = false;
                    ButtonProfileContent = "Autorize";
                    MenuItemHeader = string.Empty;
                    break;
            }
            await ChangeProductList();
            categoryItems = account?.CreateCategoryHierarchy();

            foreach (var category in categoryItems)
            {
                ComboBoxSortCategoriesItemsSource = new ObservableCollection<ComboBoxItem>();
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = category.CategoryName,
                    ToolTip = category.CategoryID
                };
                ComboBoxSortCategoriesItemsSource.Add(item);
            }
        }

        private async Task ChangeProductList()
        {
            ComboBoxItem selectedItem = ComboBoxSortCategoriesSelectedItem;
            ComboBoxSortCategoriesSelectedItem = null;
            ComboBoxSortGetDescriptionSelectedItem = null;
            ProductListItems = new ObservableCollection<ProductItem>();
            ProductItems = new List<ProductItem>();
            DataTable productTable = await Task.Run(() => account?.GetDataFromView("ProductCategoryDescriptionsView"));
            foreach (DataRow row in productTable.Rows)
            {
                if (row[0] != null)
                {
                    ProductItem ProductItem = new ProductItem
                    {
                        Product = new Product(
                            int.Parse(row[0].ToString()),
                            row[1].ToString(),
                            row[2].ToString(),
                            new BinaryContent(),
                            new Category(),
                            int.Parse(row[3].ToString()),
                            int.Parse(row[4].ToString()),
                            new Description()
                            )
                    };
                    ProductItems.Add(ProductItem);
                    myChangeProductItems.Add(ProductItem);
                    ProductListItems.Add(ProductItem);
                }
            }
            if (selectedItem == null)
                return;
            string selectedToolTipContent = selectedItem.ToolTip.ToString();
            SelectCategoryId(int.Parse(selectedToolTipContent));
        }

        private void SelectCategoryId(int categoryId = 0)
        {
            if (categoryId != 0)
            {
                myChangeProductItems = new List<ProductItem>();
                foreach (var ProductItem in ProductItems)
                {
                    if (ProductItem.Product.Category.IsCategoryInHierarchy(categoryId, categoryItems))
                    {
                        myChangeProductItems.Add(ProductItem);
                    }
                }
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems);
            }
        }
        private void ComboBoxSortCategoriesSelectionChanged(ComboBoxItem selectedItem)
        {
            if (selectedItem == null)
                return;
            ComboBoxSortGetDescriptionSelectedItem = null;
            string selectedToolTipContent = selectedItem.ToolTip.ToString();
            SelectCategoryId(int.Parse(selectedToolTipContent));
        }
        private void SortByDescription(sortStyle sortGetDescription = sortStyle.cencel)
        {
            switch (sortGetDescription)
            {
                case sortStyle.up:
                    myChangeProductItems = new List<ProductItem>(ProductItems.Where(item => item.Product.Description != null));
                    ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems);
                    break;
                case sortStyle.down:
                    myChangeProductItems = new List<ProductItem>(ProductItems.Where(item => item.Product.Description == null));
                    ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems);
                    break;
                default:
                    myChangeProductItems = ProductItems;
                    ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems);
                    break;
            }
        }

        private void ComboBoxSortGetDescriptionSelectionChanged(ComboBoxItem selectedItem)
        {
            if (selectedItem == null)
                return;

            int selectedIndex = ComboBoxSortGetDescriptionItemsSource.IndexOf(selectedItem);

            ComboBoxSortCategoriesSelectedItem = null;

            switch (selectedIndex)
            {
                case 1:
                    SortByDescription(sortStyle.up);
                    break;
                case 2:
                    SortByDescription(sortStyle.down);
                    break;
                default:
                    break;
            }
        }


        private T FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            while (depObj != null)
            {
                if (depObj is T)
                {
                    return (T)depObj;
                }
                depObj = VisualTreeHelper.GetParent(depObj);
            }
            return null;
        }
        private async void AdminMouseDoubleClick()
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null && account is AdminStoreInteraction acc)
                {
                    /*Change_Data_Product changeDataProductForm = new Change_Data_Product(acc, selectedItem);
                    changeDataProductForm.ShowDialog();
                    await ChangeProductList();*/
                }
            }
        }
        private async void AdminMenuItemCommand()
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null)
                {
                    if (account is AdminStoreInteraction acc && acc.DeleteProduct(selectedItem.Product.ProductID))
                        await ChangeProductList();
                }
            }
        }
        private async void ClientMouseDoubleClick()
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null && account is IStoreForClient)
                {
                    /*Change_Data_Product changeDataProductForm = new Change_Data_Product(acc, selectedItem);
                    changeDataProductForm.ShowDialog();
                    await ChangeProductList();*/
                }
            }
        }
        private async void ClientMenuItemCommand()
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null && account is IStoreForClient)
                {
                    /*if (account is AdminStoreInteraction acc && acc.DeleteProduct(selectedItem.productId))
                        await ChangeProductList();*/
                }
            }
        }

        private void AdminButtonProfile()
        {
            Navigation?.NavigateTo<MainSignViewModel>();
        }
        private void ClientButtonProdile()
        {
            Navigation?.NavigateTo<MainSignViewModel>();
        }
        private void GuestButtonAutorize()
        {
            Navigation?.NavigateTo<MainSignViewModel>();
        }
        #endregion

        #region Properties
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
        private bool _isSortedPrice = false;
        private bool isSortedPrice
        {
            get
            {
                return _isSortedPrice;
            }
            set
            {
                _isSortedPrice = value;
                if (value)
                {
                    _isSortedPopularity = false;
                }
            }
        }
        private bool _isSortedPopularity = false;
        private bool isSortedPopularity
        {
            get
            {
                return _isSortedPopularity;
            }
            set
            {
                _isSortedPopularity = value;
                if (value)
                {
                    _isSortedPrice = false;
                }
            }
        }
        private string _buttonProfileContent;
        public string ButtonProfileContent
        {
            get { return _buttonProfileContent; }
            set { _buttonProfileContent = value; OnPropertyChanged(nameof(ButtonProfileContent)); }
        }
        private bool _buttonCartIsEnabled;
        public bool ButtonCartIsEnabled
        {
            get { return _buttonCartIsEnabled; }
            set { _buttonCartIsEnabled = value; OnPropertyChanged(nameof(ButtonCartIsEnabled)); }
        }
        private bool _buttonExitIsEnabled;
        public bool ButtonExitIsEnabled
        {
            get { return _buttonExitIsEnabled; }
            set { _buttonExitIsEnabled = value; OnPropertyChanged(nameof(ButtonExitIsEnabled)); }
        }
        private bool _menuItemIsEnabled;
        public bool MenuItemIsEnabled
        {
            get { return _menuItemIsEnabled; }
            set { _menuItemIsEnabled = value; OnPropertyChanged(nameof(MenuItemIsEnabled)); }
        }
        private string _menuItemHeader;
        public string MenuItemHeader
        {
            get { return _menuItemHeader; }
            set { _menuItemHeader = value; OnPropertyChanged(nameof(MenuItemHeader)); }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxSortCategoriesItemsSource;
        public ObservableCollection<ComboBoxItem> ComboBoxSortCategoriesItemsSource
        {
            get { return _comboBoxSortCategoriesItemsSource; }
            set { _comboBoxSortCategoriesItemsSource = value; OnPropertyChanged(nameof(ComboBoxSortCategoriesItemsSource)); }
        }

        private ComboBoxItem _comboBoxSortCategoriesSelectedItem;
        public ComboBoxItem ComboBoxSortCategoriesSelectedItem
        {
            get { return _comboBoxSortCategoriesSelectedItem; }
            set 
            { 
                _comboBoxSortCategoriesSelectedItem = value;
                ComboBoxSortCategoriesSelectionChanged(value);
                OnPropertyChanged(nameof(ComboBoxSortCategoriesSelectedItem)); 
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxSortGetDescriptionItemsSource;
        public ObservableCollection<ComboBoxItem> ComboBoxSortGetDescriptionItemsSource
        {
            get { return _comboBoxSortGetDescriptionItemsSource; }
            set { _comboBoxSortGetDescriptionItemsSource = value; OnPropertyChanged(nameof(ComboBoxSortGetDescriptionItemsSource)); }
        }

        private ComboBoxItem _comboBoxSortGetDescriptionSelectedItem;
        public ComboBoxItem ComboBoxSortGetDescriptionSelectedItem
        {
            get { return _comboBoxSortGetDescriptionSelectedItem; }
            set { 
                _comboBoxSortGetDescriptionSelectedItem = value;
                ComboBoxSortGetDescriptionSelectionChanged(value);
                OnPropertyChanged(nameof(ComboBoxSortGetDescriptionSelectedItem)); 
            }
        }

        private ObservableCollection<ProductItem> _productListItems;
        public ObservableCollection<ProductItem> ProductListItems
        {
            get { return _productListItems; }
            set { _productListItems = value; OnPropertyChanged(nameof(ProductListItems)); }
        }

        #endregion

        #region Commands

        public ICommand ButtonProfileCommand { get; }
        public ICommand ButtonSortPriceCommand { get; }
        public ICommand ButtonSortPopularityCommand { get; }
        public ICommand ButtonUpdateCommand { get; }
        public ICommand MenuItemCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }

        private void MouseDoubleClick(object parameter)
        {
            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    AdminMouseDoubleClick();
                    break;
                case Role.client:
                    ClientMouseDoubleClick();
                    break;
            }
        }

        private void ButtonProfile(object parameter)
        {
            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    AdminButtonProfile();
                    break;
                case Role.client:
                    ClientButtonProdile();
                    break;
                case Role.guest:
                    GuestButtonAutorize();
                    break;
            }
        }
        private void ButtonSortPrice(object parameter)
        {
            if (isSortedPrice)
            {
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems.OrderBy(item => item.Product.Price));
                isSortedPrice = !isSortedPrice;
            }
            else
            {
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems.OrderByDescending(item => item.Product.Price));
                isSortedPrice = !isSortedPrice;
            }
        }

        private void ButtonSortPopularity(object parameter)
        {
            if (isSortedPopularity)
            {
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems.OrderBy(item => item.Product.SalesCount));
                isSortedPopularity = !isSortedPopularity;
            }
            else
            {
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems.OrderByDescending(item => item.Product.SalesCount));
                isSortedPopularity = !isSortedPopularity;
            }
        }

        private async void ButtonUpdate(object parameter)
        {
            await ChangeProductList();
        }

        private void ExecuteMenuItemCommand(object parameter)
        {
            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    AdminMenuItemCommand();
                    break;
                case Role.client:
                    ClientMenuItemCommand();
                    break;
            }
        }
        private bool CanExecuteMenuItem(object parameter)
        {
            return account?.user?.UserRole == Role.admin || account?.user?.UserRole == Role.client;
        }

        #endregion
    }
}
