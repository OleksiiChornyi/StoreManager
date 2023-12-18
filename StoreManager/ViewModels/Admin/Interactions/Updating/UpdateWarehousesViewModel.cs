using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using StoreManager.Views.Admin.Interactions.Updating;
using System.Data;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateWarehousesViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateWarehousesView updateWarehousesView;

        public UpdateWarehousesViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateWarehouseButtonCommand = new RelayCommand(UpdateWarehouse);

            LoadData();

            updateWarehousesView = new UpdateWarehousesView()
            {
                DataContext = this
            };
            updateWarehousesView.Show();
        }
        private void LoadData()
        {
            DataTable suppliesrsTable = _admin.GetDataFromView("UpdateWarehousesView");
            List<Warehouse> warehouses = new List<Warehouse>();
            foreach (DataRow row in suppliesrsTable.Rows)
            {
                Warehouse warehouse = new Warehouse()
                {
                    WarehoseID = int.Parse(row[0].ToString()),
                    WarehoseName = row[1].ToString(),
                    Location = row[2].ToString(),
                    Capacity = int.Parse(row[3].ToString()),
                    Availability = int.Parse(row[4].ToString())
                };
                warehouses.Add(warehouse);
            }
            foreach (var warehouse in warehouses)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = warehouse;
                item.Content = warehouse.WarehoseName;
                ComboBoxUpdateItemSource.Add(item);
            }
        }
        #region Properties
        private string _warehousesNameText;
        private string _warehousesLocationText;
        private string _warehousesCapacityText;
        private string _warehousesAvailabilityText;
        private ObservableCollection<ComboBoxItem> _comboBoxUpdateItemSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxUpdateItemSource
        {
            get => _comboBoxUpdateItemSource;
            set
            {
                if (_comboBoxUpdateItemSource != value)
                {
                    _comboBoxUpdateItemSource = value;
                    OnPropertyChanged(nameof(ComboBoxUpdateItemSource));
                }
            }
        }

        private ComboBoxItem _comboBoxUpdateSelectedItem;
        public ComboBoxItem ComboBoxUpdateSelectedItem
        {
            get => _comboBoxUpdateSelectedItem;
            set
            {
                if (_comboBoxUpdateSelectedItem != value)
                {
                    Warehouse warehouse = value.Tag as Warehouse;
                    WarehousesNameText = warehouse.WarehoseName;
                    WarehousesLocationText = warehouse.Location;
                    WarehousesAvailabilityText = warehouse.Availability.ToString();
                    WarehousesCapacityText = warehouse.Capacity.ToString();

                    _comboBoxUpdateSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxUpdateSelectedItem));
                }
            }
        }
        public string WarehousesNameText
        {
            get { return _warehousesNameText; }
            set => SetProperty(ref _warehousesNameText, value, nameof(WarehousesNameText));
        }

        public string WarehousesLocationText
        {
            get { return _warehousesLocationText; }
            set => SetProperty(ref _warehousesLocationText, value, nameof(WarehousesLocationText));
        }

        public string WarehousesCapacityText
        {
            get { return _warehousesCapacityText; }
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _warehousesCapacityText, value, nameof(WarehousesCapacityText));
                }
            }
        }

        public string WarehousesAvailabilityText
        {
            get { return _warehousesAvailabilityText; }
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _warehousesAvailabilityText, value, nameof(WarehousesAvailabilityText));
                }
            }
        }
        #endregion

        #region Commands
        public ICommand UpdateWarehouseButtonCommand { get; }

        private void UpdateWarehouse(object parameter)
        {
            Warehouse warehouse = ComboBoxUpdateSelectedItem.Tag as Warehouse;
            warehouse.WarehoseName = WarehousesNameText;
            warehouse.Location = WarehousesLocationText;
            warehouse.Capacity = int.Parse(WarehousesCapacityText);
            warehouse.Availability = int.Parse(WarehousesAvailabilityText);
            if (_admin.UpdateWareHouse(warehouse))
            {
                MessageBox.Show("Warehouse updated");
                updateWarehousesView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update warehouses");
            }
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