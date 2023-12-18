using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
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
using StoreManager.DB_classes;
using System.Runtime.Serialization.Formatters;
using StoreManager.Views.Admin.Interactions.Updating;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateInventoryViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateInventoryView updateInventoryView;

        public UpdateInventoryViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateInventoryButtonCommand = new RelayCommand(CreateInventory, CanCreateInventory);

            LoadData();

            updateInventoryView = new UpdateInventoryView()
            {
                DataContext = this
            };
            updateInventoryView.Show();
        }

        private void LoadData()
        {
            DataTable inventoryTable = _admin.GetDataFromView("InventoryUpdateView");
            List<Inventory> inventories = new List<Inventory>();
            foreach (DataRow row in inventoryTable.Rows)
            {
                Inventory inventory = new Inventory() { InventoryID = int.Parse(row[0].ToString()), ProductID = int.Parse(row[1].ToString()), QuantityOnHand = int.Parse(row[2].ToString()), WareHouseID = int.Parse(row[3].ToString()), InventoryDate = DateTime.Parse(row[4].ToString()) };
                Product product = new Product() { ProductID = inventory.ProductID, ProductName = row[5].ToString() };
                Warehouse warehouse = new Warehouse() { WarehoseID = inventory.WareHouseID, WarehoseName = row[6].ToString() };
                ComboBoxItem inv = new ComboBoxItem();
                inv.Content = product.ProductName + ", " + warehouse.WarehoseName + ", " + inventory.InventoryDate;
                inv.Tag = inventory;
                ComboBoxInventotyItemsSource.Add(inv);
                ComboBoxItem prod = new ComboBoxItem();
                prod.Content = product.ProductName;
                prod.Tag = product;
                ComboBoxProductNameItemsSource.Add(prod);
                ComboBoxItem wareh = new ComboBoxItem();
                wareh.Content = warehouse.WarehoseName;
                wareh.Tag = warehouse;
                ComboBoxWarehousesNameItemsSource.Add(wareh);
            }
        }

        #region Properties

        private ObservableCollection<ComboBoxItem> _comboBoxProductNameItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxProductNameItemsSource
        {
            get => _comboBoxProductNameItemsSource;
            set
            {
                if (_comboBoxProductNameItemsSource != value)
                {
                    _comboBoxProductNameItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxProductNameItemsSource));
                }
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxWarehousesNameItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxWarehousesNameItemsSource
        {
            get => _comboBoxWarehousesNameItemsSource;
            set
            {
                if (_comboBoxWarehousesNameItemsSource != value)
                {
                    _comboBoxWarehousesNameItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxWarehousesNameItemsSource));
                }
            }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxInventotyItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxInventotyItemsSource
        {
            get => _comboBoxInventotyItemsSource;
            set
            {
                if (_comboBoxInventotyItemsSource != value)
                {
                    _comboBoxInventotyItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxInventotyItemsSource));
                }
            }
        }
        private ComboBoxItem _comboBoxInventotySelectedItem;
        public ComboBoxItem ComboBoxInventotySelectedItem
        {
            get => _comboBoxInventotySelectedItem;
            set
            {
                if (_comboBoxInventotySelectedItem != value)
                {
                    Inventory inventory = value.Tag as Inventory;

                    ComboBoxItem selectItem = ComboBoxWarehousesNameItemsSource
                        .FirstOrDefault(item => (item.Tag as Warehouse)?.WarehoseID == inventory.WareHouseID);
                    ComboBoxWarehousesSelectedItem = selectItem;

                    selectItem = ComboBoxProductNameItemsSource
                        .FirstOrDefault(item => (item.Tag as Product)?.ProductID == inventory.ProductID);
                    ComboBoxProductNameSelectedItem = selectItem;

                    QuantityOnHandText = inventory.QuantityOnHand.ToString();

                    _comboBoxInventotySelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxInventotySelectedItem));
                }
            }
        }

        private string _quantityOnHandText;
        public string QuantityOnHandText
        {
            get => _quantityOnHandText;
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _quantityOnHandText, value, nameof(QuantityOnHandText));
                }
            }
        }

        private ComboBoxItem _comboBoxProductNameSelectedItem;
        public ComboBoxItem ComboBoxProductNameSelectedItem
        {
            get => _comboBoxProductNameSelectedItem;
            set
            {
                if (_comboBoxProductNameSelectedItem != value)
                {
                    _comboBoxProductNameSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxProductNameSelectedItem));
                }
            }
        }

        private ComboBoxItem _comboBoxWarehousesSelectedItem;
        public ComboBoxItem ComboBoxWarehousesSelectedItem
        {
            get => _comboBoxWarehousesSelectedItem;
            set
            {
                if (_comboBoxWarehousesSelectedItem != value)
                {
                    _comboBoxWarehousesSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxWarehousesSelectedItem));
                }
            }
        }

        #endregion


        #region Commands

        public ICommand CreateInventoryButtonCommand { get; }

        private void CreateInventory(object parameter)
        {
            Inventory inventory = ComboBoxInventotySelectedItem.Tag as Inventory;
            var selectedProduct = ComboBoxProductNameSelectedItem.Tag as Product;
            var selectedWarehouse = ComboBoxWarehousesSelectedItem.Tag as Warehouse;
            inventory.WareHouseID = selectedWarehouse.WarehoseID;
            inventory.ProductID = selectedProduct.ProductID;
            inventory.QuantityOnHand = int.Parse(QuantityOnHandText);
            if (_admin.UpdateInventory(inventory))
            { 
                MessageBox.Show("Inventory updated");
                updateInventoryView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update this inventory");
            }
        }

        private bool CanCreateInventory(object parameter)
        {
            return !string.IsNullOrEmpty(QuantityOnHandText) &&
                   ComboBoxProductNameSelectedItem != null &&
                   ComboBoxWarehousesSelectedItem != null;
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
