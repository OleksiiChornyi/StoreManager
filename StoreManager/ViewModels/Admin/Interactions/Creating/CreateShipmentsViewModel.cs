using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class CreateShipmentsViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly CreateShipmentsView createShipmentsView;
        public CreateShipmentsViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateShipmentsButtonCommand = new RelayCommand(CreateShipments, CanCreateShipments);

            LoadData();

            createShipmentsView = new CreateShipmentsView()
            {
                DataContext = this
            };
            createShipmentsView.Show();
        }

        private void LoadData()
        {
            DataTable ordersTable = _admin.GetDataFromView("ShipmentsOrdersView");
            foreach (DataRow row in ordersTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = row[1],
                    ToolTip = row[0]
                };
                ComboBoxOrdersItemsSource.Add(item);
            }
        }

        #region Properties
        private ObservableCollection<ComboBoxItem> _comboBoxOrdersItemsSource;
        public ObservableCollection<ComboBoxItem> ComboBoxOrdersItemsSource
        {
            get => _comboBoxOrdersItemsSource;
            set
            {
                _comboBoxOrdersItemsSource = value;
                OnPropertyChanged(nameof(ComboBoxOrdersItemsSource));
            }
        }

        private ComboBoxItem _comboBoxOrdersSelectedItem;
        public ComboBoxItem ComboBoxOrdersSelectedItem
        {
            get => _comboBoxOrdersSelectedItem;
            set
            {
                _comboBoxOrdersSelectedItem = value;
                OnPropertyChanged(nameof(ComboBoxOrdersSelectedItem));
                CreateShipmentsButtonCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<ComboBoxItem> _comboBoxStatusItemsSource;
        public ObservableCollection<ComboBoxItem> ComboBoxStatusItemsSource
        {
            get => _comboBoxStatusItemsSource;
            set
            {
                _comboBoxStatusItemsSource = value;
                OnPropertyChanged(nameof(ComboBoxStatusItemsSource));
            }
        }

        private ComboBoxItem _comboBoxStatusSelectedItem;
        public ComboBoxItem ComboBoxStatusSelectedItem
        {
            get => _comboBoxStatusSelectedItem;
            set
            {
                _comboBoxStatusSelectedItem = value;
                OnPropertyChanged(nameof(ComboBoxStatusSelectedItem));
                CreateShipmentsButtonCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand CreateShipmentsButtonCommand { get; }

        private void CreateShipments(object parameter)
        {
            if (ComboBoxOrdersSelectedItem != null)
            {
                int orderId = int.Parse(ComboBoxOrdersSelectedItem.ToolTip.ToString());

                if (ComboBoxStatusSelectedItem != null)
                {
                    string status = ComboBoxStatusSelectedItem.Content.ToString();
                    int? shipmentId = _admin.HasShipment(orderId);
                    if (shipmentId == null)
                    {
                        _admin.CreateShipments(status, orderId);
                        MessageBox.Show("Shipment created");
                    }
                    else
                    {
                        _admin.UpdateShipments(status, (int)shipmentId);
                        MessageBox.Show("Shipment updated");
                    }
                    createShipmentsView.Close();
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


        private bool CanCreateShipments(object parameter)
        {
            return ComboBoxOrdersSelectedItem != null && ComboBoxStatusSelectedItem != null;
        }
        #endregion
    }
}
