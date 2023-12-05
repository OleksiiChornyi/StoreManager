using StoreManager.Abstract.Classes;
using StoreManager.Abstract.Interfaces;
using StoreManager.Client.Cart;
using StoreManager.Guest;
using StoreManager.SignPages;
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
        private List<ImageItem> imageItems = new List<ImageItem>();
        private List<ImageItem> myChangeImageItems = new List<ImageItem>();
        private ClientStoreInteraction account;
        private StoreCartInteraction myCart;
        public ClientPage(string UserName = "Guest", string password = "1111", string contactInfo = "", Role userRole = Role.guest)
        {
            InitializeComponent();
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
                    MessageBox.Show("Користувач з таким ім'ям вже існує!");
                }
                else
                {
                    MessageBox.Show("Виникла якась помилка!\nСпробуйте ще раз");
                }
            }
            else
            {
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
                        ImageBytes = (byte[])row[2],
                        price = int.Parse(row[3].ToString()),
                        categoryId = int.Parse(row[4].ToString()),
                        categoryName = row[5].ToString(),
                        categoryDescription = row[6].ToString(),
                        salesCount = int.Parse(row[7].ToString())
                    };
                    if (row[8] == null)
                        imageItem.descriptionId = null;
                    else
                        imageItem.descriptionId = int.Parse(row[8].ToString());
                    imageItems.Add(imageItem);
                    myChangeImageItems.Add(imageItem);
                    ((ObservableCollection<ImageItem>)ProductList.ItemsSource).Add(imageItem);
                }
            }
        }

        private void SelectCategoryId(int categoryId = 0)
        {
            if (categoryId != 0)
            {
                myChangeImageItems = new List<ImageItem>(imageItems.Where(item => item.categoryId == categoryId));
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
                myCart.AddOrUpdateItem(selectedItem.productId, selectedItem.Name, selectedItem.categoryName, 1);
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
    public class ImageItem : INotifyPropertyChanged
    {
        public int productId { get; set; }
        /// <summary>
        /// Text of item
        /// </summary>
        public string Name { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryDescription { get; set; }
        public int price { get; set; }
        public int salesCount { get; set; }
        public int? descriptionId { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        private byte[] imageBytes;

        public byte[] ImageBytes
        {
            get { return imageBytes; }
            set
            {
                imageBytes = value;
                OnPropertyChanged(nameof(Image_src));
            }
        }

        public BitmapImage Image_src
        {
            get
            {
                return Convert(ImageBytes);
            }
            set
            { }
        }

        private BitmapImage Convert(byte[] bytes)
        {
            try
            {
                var image = new BitmapImage();
                using (var stream = new MemoryStream(bytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                return image;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
