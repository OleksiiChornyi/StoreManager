using StoreManager.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Views.Admin.Interactions.Creating;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class CreateWarehousesViewModel : ViewModelBase
    {
        private readonly AdminStoreInteraction _admin;
        private readonly CreateWarehousesView createWarehousesView;

        public CreateWarehousesViewModel(AdminStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateWarehouseButtonCommand = new RelayCommand(CreateWarehouse, CanCreateWarehouse);

            createWarehousesView = new CreateWarehousesView()
            {
                DataContext = this
            };
            createWarehousesView.Show();
        }
        #region Properties
        private string _warehousesNameText;
        private string _warehousesLocationText;
        private string _warehousesCapacityText;
        private string _warehousesAvailabilityText;

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
        public ICommand CreateWarehouseButtonCommand { get; }

        private void CreateWarehouse(object parameter)
        {
            _admin.CreateWarehouses(WarehousesNameText, WarehousesLocationText, int.Parse(WarehousesCapacityText), int.Parse(WarehousesAvailabilityText));
            MessageBox.Show("Warehouse added");
            createWarehousesView.Close();
        }

        private bool CanCreateWarehouse(object parameter)
        {
            return !string.IsNullOrWhiteSpace(WarehousesNameText) && !string.IsNullOrWhiteSpace(WarehousesLocationText) && !string.IsNullOrWhiteSpace(WarehousesCapacityText) && !string.IsNullOrWhiteSpace(WarehousesAvailabilityText);
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