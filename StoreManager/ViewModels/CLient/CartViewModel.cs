using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
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
using StoreManager.Models.Client.Cart;
using static StoreManager.Models.Abstract.Classes.StoreCartInteraction;

namespace StoreManager.ViewModels.CLient
{
    public class CartViewModel : ViewModelBase, IInitializable
    {
        private MyCart MyCart;
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
        public CartViewModel(INavigationService navigation)
        {
            Navigation = navigation;

            MenuItemDeleteOrderItemCommand = new RelayCommand(MenuItemDeleteOrderItem);
            ConfirmOrderCommand = new RelayCommand(ConfirmOrder);
            ButtonBackCommand = new RelayCommand(ButtonBack);
        }

        public void Initialize(object parameter = null)
        {
            MyCart = parameter as MyCart;

            LoadData();
        }

        private void LoadData()
        {
            List<StoreCartInteraction.OrderItem> orderItems = MyCart.orderItems;
            DataGridItemsSource = new List<StoreCartInteraction.OrderItem>(orderItems);
            TotalPriceText = MyCart.GetTotalPrice().ToString();
        }

        #region Properties

        private List<StoreCartInteraction.OrderItem> _dataGridItemsSource;
        public List<StoreCartInteraction.OrderItem> DataGridItemsSource
        {
            get => _dataGridItemsSource;
            set => SetProperty(ref _dataGridItemsSource, value, nameof(DataGridItemsSource));
        }

        private StoreCartInteraction.OrderItem _dataGridSelectedItem;
        public StoreCartInteraction.OrderItem DataGridSelectedItem
        {
            get => _dataGridSelectedItem;
            set
            {
                _dataGridSelectedItem = value;
                OnPropertyChanged(nameof(DataGridSelectedItem));
            }
        }

        private string _cardNumberText;
        public string CardNumberText
        {
            get => _cardNumberText;
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _cardNumberText, value, nameof(CardNumberText));
                }
            }
        }
        private string _totalPriceText;
        public string TotalPriceText
        {
            get => _totalPriceText;
            set
            {
                SetProperty(ref _totalPriceText, value, nameof(TotalPriceText));
            }
        }
        private bool _isCardIsChecked;
        public bool IsCardIsChecked
        {
            get => _isCardIsChecked;
            set
            {
                _isCardIsChecked = value;
                OnPropertyChanged(nameof(IsCardIsChecked));
            }
        }

        #endregion

        #region Commands

        public ICommand MenuItemDeleteOrderItemCommand { get; }
        public ICommand ConfirmOrderCommand { get; }
        public ICommand ButtonBackCommand { get; }

        private void MenuItemDeleteOrderItem(object parameter)
        {
            MyCart.RemoveItem(DataGridSelectedItem.product);
            LoadData();
        }
        private void ConfirmOrder(object parameter)
        {
            if (MyCart.isCreating)
            {
                MessageBox.Show("The order has already been created");
                return;
            }
            if (MyCart.CreateOrder(IsCardIsChecked, CardNumberText))
            {
                MyCart.CreateNewCart();
                MessageBox.Show("Order created");
                Navigation.GoBack(MyCart.client);
            }
            else
                MessageBox.Show("Some error created");
        }
        private void ButtonBack(object parameter)
        {
            Navigation.GoBack(MyCart.client);
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
