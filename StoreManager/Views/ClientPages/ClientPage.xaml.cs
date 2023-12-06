using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Client.Cart;
using StoreManager.Models.Guest;
using StoreManager.ViewModels.Data;
using StoreManager.Views.ClientPages;
using StoreManager.Views.SignPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoreManager.Models.Client;
using StoreManager.Models.Client.Cart;

namespace StoreManager.Client
{
    /// <summary>
    /// Interaction logic for ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private enum sortStyle { up, down, cencel }
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
        private List<CategoryItem> categoryItems = new List<CategoryItem>();
        private List<ImageItem> imageItems = new List<ImageItem>();
        private List<ImageItem> myChangeImageItems = new List<ImageItem>();
        private readonly ClientStoreInteraction account;
        private readonly StoreCartInteraction myCart;
        private readonly bool isEmulated;
        public ClientPage(string UserName = "Guest", string password = "1111", string contactInfo = "", Role userRole = Role.guest, bool isEmulated = false)
        {
            InitializeComponent();
            this.isEmulated = isEmulated;
            switch (userRole)
            {
                case Role.client:
                    account = new StoreForClient(UserName, password, contactInfo, userRole);
                    break;
                case Role.guest:
                    account = new StoreForGuest(UserName, password, contactInfo, userRole);
                    break;
                default:
                    break;
            }
            if (account == null || !account.isOk)
            {
                if (account.isExist)
                {
                    MessageBox.Show("A user with this name already exists!");
                }
                else
                {
                    MessageBox.Show("There's been an error!\nTry again");
                }
            }
            else
            {
                categoryItems = account.CreateCategoryHierarchy();
                ChangeProductList();
                myCart = new MyCart(account);

                DataTable categoriesTable = account.GetDataFromView("ProductsCategoriesView");
                foreach (DataRow row in categoriesTable.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = row[1];
                    item.ToolTip = row[0];
                    ComboBoxSortCategories.Items.Add(item);
                }
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (account.myRole == Role.guest)
            {
                ButtonCart.IsEnabled = false;
                ProductList.ContextMenu = null;
                if (isEmulated)
                {
                    ButtonExit.IsEnabled = false;
                }
            }
            else
            {
                ButtonCart.IsEnabled = true;
            }
        }

        public bool GetAutorization()
        {
            if (account == null)
                return false;
            return account.isOk;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainSignPage());
        }

        private async Task ChangeProductList()
        {
            ComboBoxSortGetDescription.SelectedIndex = -1;
            ComboBoxSortCategories.SelectedIndex = -1;
            ProductList.ItemsSource = new ObservableCollection<ImageItem>();
            imageItems = new List<ImageItem>();
            DataTable productTable = await Task.Run(() => account.GetDataFromView("ProductCategoryIdView"));
            foreach (DataRow row in productTable.Rows)
            {
                if (row[0] != null)
                {
                    ImageItem imageItem = new ImageItem
                    {
                        productId = int.Parse(row[0].ToString()),
                        Name = row[1].ToString(),
                        fileNameImage = row[2].ToString(),
                        ImageBytes = (byte[])row[3],
                        price = int.Parse(row[4].ToString()),
                        categoryId = int.Parse(row[5].ToString()),
                        categoryName = row[6].ToString(),
                        categoryDescription = row[7].ToString(),
                        salesCount = int.Parse(row[8].ToString())
                    };
                    int descriptionId;
                    if (int.TryParse(row[9].ToString(), out descriptionId))
                        imageItem.descriptionId = descriptionId;
                    else
                        imageItem.descriptionId = null;
                    imageItems.Add(imageItem);
                    myChangeImageItems.Add(imageItem);
                    ((ObservableCollection<ImageItem>)ProductList.ItemsSource).Add(imageItem);
                }
            }
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBoxSortCategories.SelectedItem;
            if (selectedItem == null)
                return;
            ComboBoxSortGetDescription.SelectedIndex = -1;
            string selectedToolTipContent = selectedItem.ToolTip.ToString();
            SelectCategoryId(int.Parse(selectedToolTipContent));
        }

        private void SelectCategoryId(int categoryId = 0)
        {
            if (categoryId != 0)
            {
                myChangeImageItems = new List<ImageItem>();
                foreach (var imageItem in imageItems)
                {
                    if (CategoryItem.IsCategoryInHierarchy(imageItem.categoryId, categoryId, categoryItems))
                    {
                        myChangeImageItems.Add(imageItem);
                    }
                }
                ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems);
            }
        }

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.Selector)sender).SelectedValue == null)
                return;
            if (!(sender is ListView listView)) return;
            var selectedItem = listView.SelectedValue;
            if (selectedItem == null) return;
            Point mousePosition = e.GetPosition(listView);
            var elementUnderMouse = listView.InputHitTest(mousePosition) as FrameworkElement;
            if (elementUnderMouse.DataContext == null || elementUnderMouse.DataContext.GetType() != typeof(ImageItem))
            {
                return;
            }
            ClientDescriptionView clientDescriptionViewForm = new ClientDescriptionView(account, myCart, (ImageItem)((System.Windows.Controls.Primitives.Selector)sender).SelectedItem);
            clientDescriptionViewForm.Show();
        }

        private void MenuItemAddshoppingCart_Click(object sender, RoutedEventArgs e)
        {
            if (account.myRole == Role.guest)
                return;

            if (ProductList.SelectedItem == null)
                return;

            var selectedItem = (ImageItem)ProductList.SelectedItem;

            if (selectedItem != null)
            {
                myCart.AddOrUpdateItem(selectedItem.productId, selectedItem.Name, selectedItem.price, selectedItem.categoryName, 1);
            }
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            await ChangeProductList();
        }

        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MyCartPage(myCart));
        }

        private void SortByDescription(sortStyle sortGetDescription = sortStyle.cencel)
        {
            switch (sortGetDescription)
            {
                case sortStyle.up:
                    myChangeImageItems = new List<ImageItem>(imageItems.Where(item => item.descriptionId != null));
                    ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems);
                    break;
                case sortStyle.down:
                    myChangeImageItems = new List<ImageItem>(imageItems.Where(item => item.descriptionId == null));
                    ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems);
                    break;
                default:
                    myChangeImageItems = imageItems;
                    ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems);
                    break;
            }
        }

        private void ButtonSortPrice_Click(object sender, RoutedEventArgs e)
        {
            if (isSortedPrice)
            {
                ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems.OrderBy(item => item.price));
                isSortedPrice = !isSortedPrice;
            }
            else
            {
                ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems.OrderByDescending(item => item.price));
                isSortedPrice = !isSortedPrice;
            }
        }

        private void ButtonSortPopularity_Click(object sender, RoutedEventArgs e)
        {
            if (isSortedPopularity)
            {
                ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems.OrderBy(item => item.salesCount));
                isSortedPopularity = !isSortedPopularity;
            }
            else
            {
                ProductList.ItemsSource = new ObservableCollection<ImageItem>(myChangeImageItems.OrderByDescending(item => item.salesCount));
                isSortedPopularity = !isSortedPopularity;
            }
        }

        private void ComboBoxSortCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBoxSortCategories.SelectedItem;
            if (selectedItem == null)
                return;
            ComboBoxSortGetDescription.SelectedIndex = -1;
            string selectedToolTipContent = selectedItem.ToolTip.ToString();
            SelectCategoryId(int.Parse(selectedToolTipContent));
        }

        private void ComboBoxSortGetDescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ComboBoxSortGetDescription.SelectedIndex;
            if (selectedIndex == -1) 
                return;
            ComboBoxSortCategories.SelectedIndex = -1;
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
    }
}
