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
using StoreManager.Models.Admin;
using System.Windows.Data;
using StoreManager.Models.Client.Cart;
using StoreManager.ViewModels.CLient;
using StoreManager.Models.Manager;

namespace StoreManager.ViewModels.StoreInteraction
{
    internal class MainStoreInterationViewModel : ViewModelBase, IInitializable
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
        private enum sortStyle { up, down, cencel }
        private AllUsersInteractions account;
        private StoreCartInteraction myCart = null;
        private List<Category> categoryItems = new List<Category>();
        private readonly List<ProductItem> StaticProductItems = new List<ProductItem>();
        private List<ProductItem> myChangeProductItems = new List<ProductItem>();
        private int? _selectedCategoryID = null;
        public MainStoreInterationViewModel(INavigationService navigation)
        {
            Navigation = navigation;

            ButtonProfileCommand = new RelayCommand(ButtonProfile);
            ButtonSortPriceCommand = new RelayCommand(ButtonSortPrice);
            ButtonSortPopularityCommand = new RelayCommand(ButtonSortPopularity);
            ButtonUpdateCommand = new RelayCommand(ButtonUpdate);
            ButtonCartCommand = new RelayCommand(ButtonCart);
            MenuItemAddToCartItemCommand = new RelayCommand(MenuItemAddToCartItem, CanExecuteMenuItemAddToCartItem);
            MenuItemDeleteItemItemCommand = new RelayCommand(MenuItemDeleteItemItem, CanExecuteMenuItemDeleteItemItem);
            MouseDoubleClickCommand = new RelayCommand(MouseDoubleClick);
            MenuButtonAllProductsCommand = new RelayCommand(MenuButtonAllProducts);
        }
        private ContextMenu _contextMenu;
        public ContextMenu ContextMenu
        {
            get { return _contextMenu; }
            set 
            {
                _contextMenu = value; 
                OnPropertyChanged(nameof(ContextMenu));
            }
        }
        public void Initialize(object parameter)
        {
            if (parameter == null)
            {
                Checkings.CreateGuestIfNotExist();
                account = new StoreForGuest();
                myCart = null;
            }
            else if (parameter is User)
            {
                User user = parameter as User;
                switch (user.UserRole)
                {
                    case Role.admin:
                        account = new StoreManagerForAdmin(user);
                        break;
                    case Role.manager:
                        account = new StoreManagerForManager(user);
                        break;
                    case Role.client:
                        account = new StoreForClient(user, true);
                        break;
                    default:
                        account = new StoreForGuest();
                        break;
                }
            }
            else
            {
                int? userId = account?.user?.UserID;
                account = parameter as AllUsersInteractions;
                if (account.user.UserID != userId)
                {
                    switch (account?.user?.UserRole)
                    {
                        case Role.admin:
                            myCart = new MyCart(account);
                            break;
                        case Role.manager:
                            myCart = new MyCart(account);
                            break;
                        case Role.client:
                            myCart = new MyCart(account);
                            break;
                        case Role.guest:
                            myCart = null;
                            break;
                        default:
                            myCart = null;
                            break;
                    }
                }
            }

            LoadData();
        }

        #region Functions
        private async void LoadData()
        {
            IsEnabledButtonProfile = true;

            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    ButtonCartIsEnabled = true;
                    MenuItemAddToCartVisibility = Visibility.Visible;
                    MenuItemDeleteItemVisibility = Visibility.Visible;
                    ButtonProfileContent = "Profile";
                    break;
                case Role.manager:
                    ButtonCartIsEnabled = true;
                    MenuItemAddToCartVisibility = Visibility.Visible;
                    //MenuItemDeleteItemVisibility = Visibility.Visible;
                    ButtonProfileContent = "Profile";
                    break;
                case Role.client:
                    ButtonCartIsEnabled = true;
                    MenuItemAddToCartVisibility = Visibility.Visible;
                    MenuItemDeleteItemVisibility = Visibility.Collapsed;
                    ButtonProfileContent = "Profile";
                    break;
                case Role.guest:
                    ButtonCartIsEnabled = false;
                    MenuItemAddToCartVisibility = Visibility.Collapsed;
                    MenuItemDeleteItemVisibility = Visibility.Collapsed;
                    ButtonProfileContent = "Autorize";
                    IsEnabledButtonProfile = !Navigation.isEmulated;
                    break;
                default:
                    ButtonCartIsEnabled = false;
                    MenuItemAddToCartVisibility = Visibility.Collapsed;
                    MenuItemDeleteItemVisibility = Visibility.Collapsed;
                    ButtonProfileContent = "Autorize";
                    IsEnabledButtonProfile = !Navigation.isEmulated;
                    break;
            }
            if (myCart == null)
                switch (account?.user?.UserRole)
                {
                    case Role.admin:
                        myCart = new MyCart(account);
                        break;
                    case Role.manager:
                        myCart = new MyCart(account);
                        break;
                    case Role.client:
                        myCart = new MyCart(account);
                        break;
                    case Role.guest:
                        myCart = null;
                        break;
                    default:
                        myCart = null;
                        break;
                }
            await ChangeProductList();
        }

        private async Task ChangeProductList()
        {
            _selectedCategoryID = null;
            ComboBoxSortGetDescriptionSelectedItem = null;
            StaticProductItems.Clear();
            ProductListItems = new ObservableCollection<ProductItem>();
            List<Product> products = await Task.Run(() => account?.GetProductDataFromDatabase());
            foreach (Product product in products)
            {
                ProductItem ProductItem = new ProductItem(product);
                StaticProductItems.Add(ProductItem);
                myChangeProductItems.Add(ProductItem);
                ProductListItems.Add(ProductItem);
            }

            categoryItems = account?.CreateCategoryHierarchy();
            var groupedCategories = categoryItems.GroupBy(c => c.ParentCategoryID);
            PopulateMenuItemSortCategories(groupedCategories);
        }

        public void PopulateMenuItemSortCategories(IEnumerable<IGrouping<int?, Category>> groupedCategories)
        {
            var rootCategories = groupedCategories.FirstOrDefault(g => !g.Key.HasValue);

            if (rootCategories == null)
            {
                MenuItemSortCategories = new ObservableCollection<MenuItem>();
            }
            else
            {
                var rootMenuItems = rootCategories.Select(rootCategory => CreateMenuItem(rootCategory, groupedCategories));

                MenuItemSortCategories = new ObservableCollection<MenuItem>(rootMenuItems);
            }
        }

        private MenuItem CreateMenuItem(Category category, IEnumerable<IGrouping<int?, Category>> groupedCategories)
        {
            var menuItem = new MenuItem
            {
                Header = category.CategoryName,
                Tag = category
            };
            menuItem.PreviewMouseLeftButtonDown += SelectNewCategory;

            var subCategories = groupedCategories.FirstOrDefault(g => g.Key == category.CategoryID);

            if (subCategories != null)
            {
                foreach (var subCategory in subCategories)
                {
                    var subMenuItem = CreateMenuItem(subCategory, groupedCategories);
                    menuItem.Items.Add(subMenuItem);
                }
            }

            return menuItem;
        }

        private void SelectCategoryId()
        {
            if (_selectedCategoryID != null)
            {
                myChangeProductItems = new List<ProductItem>();
                foreach (var ProductItem in StaticProductItems)
                {
                    if (ProductItem.Product.Category.IsCategoryInHierarchy((int)_selectedCategoryID, categoryItems))
                    {
                        myChangeProductItems.Add(ProductItem);
                    }
                }
            }
            else
            {
                myChangeProductItems = new List<ProductItem>(StaticProductItems);
            }
            ProductListItems = new ObservableCollection<ProductItem>(ComboBoxSortGetDescriptionSelectionChanged(myChangeProductItems));
        }
        private List<ProductItem> ComboBoxSortGetDescriptionSelectionChanged(List<ProductItem> items)
        {
            if (ComboBoxSortGetDescriptionSelectedItem == null)
                return items;

            int selectedIndex = ComboBoxSortGetDescriptionItemsSource.IndexOf(ComboBoxSortGetDescriptionSelectedItem);

            _selectedCategoryID = null;

            switch (selectedIndex)
            {
                case 1:
                    items = SortByDescription(items, sortStyle.up);
                    break;
                case 2:
                    items = SortByDescription(items, sortStyle.down);
                    break;
                default:
                    break;
            }
            return items;
        }
        private List<ProductItem> SortByDescription(List<ProductItem> items, sortStyle sortGetDescription)
        {
            switch (sortGetDescription)
            {
                case sortStyle.up:
                    items = new List<ProductItem>(items.Where(item => item.Product.Description != null));
                    break;
                case sortStyle.down:
                    items = new List<ProductItem>(items.Where(item => item.Product.Description == null));
                    break;
                default:
                    items = items.ToList();
                    break;
            }
            return items;
        }
        private void SortByDescription(sortStyle sortGetDescription = sortStyle.cencel)
        {
            ProductListItems = new ObservableCollection<ProductItem>();
            switch (sortGetDescription)
            {
                case sortStyle.up:
                    myChangeProductItems = new List<ProductItem>(StaticProductItems.Where(item => item.Product.Description != null));
                    break;
                case sortStyle.down:
                    myChangeProductItems = new List<ProductItem>(StaticProductItems.Where(item => item.Product.Description == null));
                    break;
                default:
                    myChangeProductItems = StaticProductItems.ToList();
                    break;
            }
            if (_selectedCategoryID != null)
            {
                foreach (var ProductItem in myChangeProductItems)
                {
                    if (ProductItem.Product.Category.IsCategoryInHierarchy((int)_selectedCategoryID, categoryItems))
                    {
                        ProductListItems.Add(ProductItem);
                    }
                }
            }
            else
            {
                ProductListItems = new ObservableCollection<ProductItem>(myChangeProductItems);
            }
        }

        private void ComboBoxSortGetDescriptionSelectionChanged()
        {
            if (ComboBoxSortGetDescriptionSelectedItem == null)
                return;

            int selectedIndex = ComboBoxSortGetDescriptionItemsSource.IndexOf(ComboBoxSortGetDescriptionSelectedItem);

            switch (selectedIndex)
            {
                case 1:
                    SortByDescription(sortStyle.up);
                    break;
                case 2:
                    SortByDescription(sortStyle.down);
                    break;
                default:
                    SortByDescription(sortStyle.cencel);
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
        private async void ClientMouseDoubleClick()
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null)
                {
                    new ProductDescriptionViewModel(myCart, selectedItem);
                }
            }
        }

        private void AdminButtonProfile()
        {
            Navigation?.NavigateTo<ProfileViewModel>(account);
        }
        private void ClientButtonProdile()
        {
            Navigation?.NavigateTo<ProfileViewModel>(account);
        }
        private void GuestButtonAutorize()
        {
            Navigation?.NavigateTo<MainSignViewModel>();
        }
        #endregion

        #region Properties
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
            set { SetProperty(ref _buttonProfileContent, value, nameof(ButtonProfileContent)); }
        }
        private bool _buttonCartIsEnabled;
        public bool ButtonCartIsEnabled
        {
            get { return _buttonCartIsEnabled; }
            set { _buttonCartIsEnabled = value; OnPropertyChanged(nameof(ButtonCartIsEnabled)); }
        }
        private bool _isEnabledButtonProfile;
        public bool IsEnabledButtonProfile
        {
            get { return _isEnabledButtonProfile; }
            set { _isEnabledButtonProfile = value; OnPropertyChanged(nameof(IsEnabledButtonProfile)); }
        }
        private Visibility _menuItemAddToCartVisibility;
        public Visibility MenuItemAddToCartVisibility
        {
            get { return _menuItemAddToCartVisibility; }
            set { _menuItemAddToCartVisibility = value; OnPropertyChanged(nameof(MenuItemAddToCartVisibility)); }
        }
        private Visibility _menuItemDeleteItemVisibility;
        public Visibility MenuItemDeleteItemVisibility
        {
            get { return _menuItemDeleteItemVisibility; }
            set { _menuItemDeleteItemVisibility = value; OnPropertyChanged(nameof(MenuItemDeleteItemVisibility)); }
        }

        private ObservableCollection<MenuItem> _menuItemSortCategories = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> MenuItemSortCategories
        {
            get { return _menuItemSortCategories; }
            set { _menuItemSortCategories = value; OnPropertyChanged(nameof(MenuItemSortCategories)); }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxSortGetDescriptionItemsSource = new ObservableCollection<ComboBoxItem>(new List<ComboBoxItem>() 
        { 
            new ComboBoxItem() { Content = "All products" },
            new ComboBoxItem() { Content = "With description" },
            new ComboBoxItem() { Content = "No description" } 
        });
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
                ComboBoxSortGetDescriptionSelectionChanged();
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
        public ICommand ButtonCartCommand { get; }
        public ICommand MenuItemAddToCartItemCommand { get; }
        public ICommand MenuItemDeleteItemItemCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }
        public ICommand MenuButtonAllProductsCommand { get; }

        private void MouseDoubleClick(object parameter)
        {
            if (account?.user?.UserRole != Role.guest)
            {
                ClientMouseDoubleClick();
            }
        }

        private void ButtonProfile(object parameter)
        {
            switch (account?.user?.UserRole)
            {
                case Role.admin:
                    AdminButtonProfile();
                    break;
                case Role.manager:
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
                ProductListItems = new ObservableCollection<ProductItem>(ProductListItems.OrderBy(item => item.Product.Price));
                isSortedPrice = !isSortedPrice;
            }
            else
            {
                ProductListItems = new ObservableCollection<ProductItem>(ProductListItems.OrderByDescending(item => item.Product.Price));
                isSortedPrice = !isSortedPrice;
            }
        }

        private void ButtonSortPopularity(object parameter)
        {
            if (isSortedPopularity)
            {
                ProductListItems = new ObservableCollection<ProductItem>(ProductListItems.OrderBy(item => item.Product.SalesCount));
                isSortedPopularity = !isSortedPopularity;
            }
            else
            {
                ProductListItems = new ObservableCollection<ProductItem>(ProductListItems.OrderByDescending(item => item.Product.SalesCount));
                isSortedPopularity = !isSortedPopularity;
            }
        }

        private async void ButtonUpdate(object parameter)
        {
            await ChangeProductList();
        }

        private void ButtonCart(object parameter)
        {
            Navigation.NavigateTo<CartViewModel>(myCart);
        }

        private void MenuItemAddToCartItem(object parameter)
        {
            DependencyObject dep = (DependencyObject)Mouse.DirectlyOver;

            ListViewItem listViewItem = FindVisualParent<ListViewItem>(dep);

            if (listViewItem != null)
            {
                ProductItem selectedItem = (ProductItem)listViewItem.DataContext;

                if (selectedItem != null && account.user.UserRole != Role.guest)
                {
                    myCart.AddOrUpdateItem(selectedItem.Product, 1);
                }
            }
        }
        private async void MenuItemDeleteItemItem(object parameter)
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
        private bool CanExecuteMenuItemAddToCartItem(object parameter)
        {
            return account?.user?.UserRole == Role.admin || account?.user?.UserRole == Role.manager || account?.user?.UserRole == Role.client;
        }
        private bool CanExecuteMenuItemDeleteItemItem(object parameter)
        {
            return account?.user?.UserRole == Role.admin;
        }


        private void SelectNewCategory(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is Category category)
            {
                _selectedCategoryID = category.CategoryID;
                SelectCategoryId();
            }
        }
        private void MenuButtonAllProducts(object parameter)
        {
            _selectedCategoryID = null;
            ComboBoxSortGetDescriptionSelectedItem = null;
            SelectCategoryId();
        }
        #endregion
    }
}
