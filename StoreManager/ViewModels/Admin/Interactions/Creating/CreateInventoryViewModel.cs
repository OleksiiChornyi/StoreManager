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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class CreateInventoryViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly CreateInventoryView createInventoryView;

        public CreateInventoryViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateInventoryButtonCommand = new RelayCommand(CreateInventory, CanCreateInventory);

            LoadData();

            createInventoryView = new CreateInventoryView()
            {
                DataContext = this
            };
            createInventoryView.Show();
        }

        private void LoadData()
        {
            DataTable productTable = _admin.GetDataFromView("InventoryProductsView");
            foreach (DataRow row in productTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                ComboBoxProductNameItemsSource.Add(item);
            }

            DataTable warehousesTable = _admin.GetDataFromView("InventoryWarehousesView");
            foreach (DataRow row in warehousesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row[1];
                item.ToolTip = row[0];
                ComboBoxWarehousesNameItemsSource.Add(item);
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
            var selectedProduct = ComboBoxProductNameSelectedItem;
            if (selectedProduct != null)
            {
                int productId = int.Parse(selectedProduct.ToolTip.ToString());

                var selectedWarehouse = ComboBoxWarehousesSelectedItem;
                if (selectedWarehouse != null)
                {
                    int warehouseId = int.Parse(selectedWarehouse.ToolTip.ToString());
                    _admin.CreateInventory(productId, int.Parse(QuantityOnHandText), warehouseId);
                }
                MessageBox.Show("Inventory added");
                createInventoryView.Close();
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
