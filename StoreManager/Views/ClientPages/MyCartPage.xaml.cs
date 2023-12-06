using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static StoreManager.Models.Abstract.Classes.StoreCartInteraction;

namespace StoreManager.Client.Cart
{
    /// <summary>
    /// Interaction logic for MyCartPage.xaml
    /// </summary>
    public partial class MyCartPage : Page
    {
        readonly StoreCartInteraction myCart;
        public MyCartPage(StoreCartInteraction myCart)
        {
            InitializeComponent();
            this.myCart = myCart;
        }

        private void MenuItemDeleteOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem))
            {
                return;
            }

            if (!(menuItem.Parent is ContextMenu contextMenu))
            {
                return;
            }

            if (!(contextMenu.PlacementTarget is DataGrid dataGrid))
            {
                return;
            }

            object selectedItem = dataGrid.SelectedItem;

            if (selectedItem != null)
            {
                if (selectedItem is OrderItem selectedOrderItem)
                {
                    myCart.RemoveItem(selectedOrderItem.productId);
                    UpdateData();
                }
            }
        }

        void UpdateData()
        {
            dataGrid.ItemsSource = new List<OrderItem>();
            dataGrid.ItemsSource = myCart.orderItems;
            TotalPrice.Text = myCart.GetTotalPrice().ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (myCart.isCreating)
            {
                MessageBox.Show("The order has already been created");
                return;
            }
            myCart.CreateOrder((bool)IsCard.IsChecked, CardNumber.Text);
            myCart.CreateNewCart();
            MessageBox.Show("Order created");
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void CardNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
