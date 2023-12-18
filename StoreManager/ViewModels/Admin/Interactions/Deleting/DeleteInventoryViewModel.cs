using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Deleting;
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

namespace StoreManager.ViewModels.Admin.Interactions.Deleting
{
    public class DeleteInventoryViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly DeleteInventoryView deleteInventoryView;
        public DeleteInventoryViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();

            DeleteItemButtonCommand = new RelayCommand(DeleteItem);

            deleteInventoryView = new DeleteInventoryView()
            {
                DataContext = this
            };
            deleteInventoryView.Show();
        }
        private void LoadData()
        {
            DataTable categoriesTable = _admin.GetDataFromView("InventoryUpdateView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxCurrentItemsSource.Add(new ComboBoxItem
                {
                    Tag = row[0],
                    Content = row[1]
                });
            }
        }
        #region Properties
        private ObservableCollection<ComboBoxItem> _comboBoxCurrentItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCurrentItemsSource
        {
            get => _comboBoxCurrentItemsSource;
            set
            {
                if (_comboBoxCurrentItemsSource != value)
                {
                    _comboBoxCurrentItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxCurrentSelectedItem;
        public ComboBoxItem ComboBoxCurrentSelectedItem
        {
            get => _comboBoxCurrentSelectedItem;
            set
            {
                if (_comboBoxCurrentSelectedItem != value)
                {
                    _comboBoxCurrentSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentSelectedItem));
                }
            }
        }
        #endregion
        #region Commands
        public ICommand DeleteItemButtonCommand { get; }

        private void DeleteItem(object parameter)
        {
            int? selectyId = ComboBoxCurrentSelectedItem?.Tag != null
                ? Convert.ToInt32(ComboBoxCurrentSelectedItem.Tag)
                : (int?)null;
            if (selectyId != null)
            {
                if (_admin.DeleteInventory((int)selectyId))
                {
                    MessageBox.Show("Inventory deleted");
                    deleteInventoryView.Close();
                }
                else
                {
                    MessageBox.Show("Can`t delete this inventory");
                }
            }
        }
        #endregion
    }
}
