using StoreManager.DB_classes;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateOrdersViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateOrdersView updateOrdersView;
        public UpdateOrdersViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateShipmentsButtonCommand = new RelayCommand(CreateShipmentsButton);
            LoadData();

            updateOrdersView = new UpdateOrdersView()
            {
                DataContext = this
            };
            updateOrdersView.Show();
        }
        private void LoadData()
        {
            DataTable ordersTable = _admin.GetDataFromView("ShipmentsOrdersView");
            ComboBoxOrdersItemSource = new ObservableCollection<ComboBoxItem>();
            foreach (DataRow row in ordersTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                Order order = new Order() { OrderID = int.Parse(row[0].ToString()), OrderStatus = row[2].ToString() };
                item.Tag = order;
                item.Content = row[1].ToString();
                ComboBoxOrdersItemSource.Add(item);
            }
            ComboBoxOrdersStatusItemSource = new ObservableCollection<ComboBoxItem>()
            {
            new ComboBoxItem() { Content = "The order is leaving" },
            new ComboBoxItem() { Content = "Order on the way" },
            new ComboBoxItem() { Content = "The order has arrived" }
            };
        }
        #region Properties
        private ObservableCollection<ComboBoxItem> _comboBoxOrdersItemSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxOrdersItemSource
        {
            get => _comboBoxOrdersItemSource;
            set
            {
                if (_comboBoxOrdersItemSource != value)
                {
                    _comboBoxOrdersItemSource = value;
                    OnPropertyChanged(nameof(ComboBoxOrdersItemSource));
                }
            }
        }

        private ComboBoxItem _comboBoxOrdersSelectedItem;
        public ComboBoxItem ComboBoxOrdersSelectedItem
        {
            get => _comboBoxOrdersSelectedItem;
            set
            {
                if (_comboBoxOrdersSelectedItem != value)
                {
                    Order order = value.Tag as Order;

                    ComboBoxItem selectItem = ComboBoxOrdersStatusItemSource
                        .FirstOrDefault(item => (item.Content as string) == order.OrderStatus);
                    ComboBoxOrdersStatusSelectedItem = selectItem;

                    _comboBoxOrdersSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxOrdersSelectedItem));
                }
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxOrdersStatusItemSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxOrdersStatusItemSource
        {
            get => _comboBoxOrdersStatusItemSource;
            set
            {
                if (_comboBoxOrdersStatusItemSource != value)
                {
                    _comboBoxOrdersStatusItemSource = value;
                    OnPropertyChanged(nameof(ComboBoxOrdersStatusItemSource));
                }
            }
        }

        private ComboBoxItem _comboBoxOrdersStatusSelectedItem;
        public ComboBoxItem ComboBoxOrdersStatusSelectedItem
        {
            get => _comboBoxOrdersStatusSelectedItem;
            set
            {
                if (_comboBoxOrdersStatusSelectedItem != value)
                {
                    _comboBoxOrdersStatusSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxOrdersStatusSelectedItem));
                }
            }
        }
        #endregion
        #region Commands
        public ICommand CreateShipmentsButtonCommand { get; }

        private void CreateShipmentsButton(object parameter)
        {
            if (ComboBoxOrdersSelectedItem is ComboBoxItem selected)
            {
                Order order = selected.Tag as Order;
                int OrderId = order.OrderID;

                selected = ComboBoxOrdersStatusSelectedItem;
                if (selected != null)
                {
                    string status = selected.Content.ToString();
                    int? shipmentId = _admin.HasShipment(OrderId);
                    if (shipmentId == null)
                    {
                        _admin.CreateShipments(status, OrderId);
                        MessageBox.Show("Shipment created");
                    }
                    else
                    {
                        _admin.UpdateShipments(status, (int)shipmentId);
                        MessageBox.Show("Shipment updated");
                    }
                    updateOrdersView.Close();
                }
                else
                {
                    MessageBox.Show("An error occurred");
                }
            }
            else
            {
                MessageBox.Show("An error occurred");
            }
        }
        #endregion
    }
}
