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
using StoreManager.DB_classes;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using StoreManager.Views.Admin.Interactions.Updating;
using System.Data;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateSupplierViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateSupplierView updateSupplierView;

        public UpdateSupplierViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateSuppliersButtonCommand = new RelayCommand(UpdateSuppliers, CanCreateSuppliers);

            LoadData();

            updateSupplierView = new UpdateSupplierView()
            {
                DataContext = this
            };
            updateSupplierView.Show();
        }
        private void LoadData()
        {
            DataTable suppliesrsTable = _admin.GetDataFromView("UpdateSuppliersView");
            List<Supplier> suppliers = new List<Supplier>();
            foreach (DataRow row in suppliesrsTable.Rows)
            {
                Supplier supplier = new Supplier()
                {
                    SupplierID = int.Parse(row[0].ToString()),
                    CompanyName = row[1].ToString(),
                    ContactInfo = row[2].ToString(),
                    SupplierAddress = row[3].ToString()
                };
                suppliers.Add(supplier);
            }
            foreach (var supplier in suppliers)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = supplier;
                item.Content = supplier.CompanyName;
                ComboBoxUpdateItemSource.Add(item);
            }
        }
        #region Properties
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
                    Supplier supplier = value.Tag as Supplier;
                    SuppliersNameText = supplier.CompanyName;
                    SuppliersContactInfoText = supplier.ContactInfo;
                    SuppliersAddressText = supplier.SupplierAddress;

                    _comboBoxUpdateSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxUpdateSelectedItem));
                }
            }
        }
        private string _suppliersNameText;
        private string _suppliersContactInfoText;
        private string _suppliersAddressText;

        public string SuppliersNameText
        {
            get { return _suppliersNameText; }
            set => SetProperty(ref _suppliersNameText, value, nameof(SuppliersNameText));
        }

        public string SuppliersContactInfoText
        {
            get { return _suppliersContactInfoText; }
            set => SetProperty(ref _suppliersContactInfoText, value, nameof(SuppliersContactInfoText));
        }

        public string SuppliersAddressText
        {
            get { return _suppliersAddressText; }
            set => SetProperty(ref _suppliersAddressText, value, nameof(SuppliersAddressText));
        }
        #endregion
        #region Commands
        public ICommand UpdateSuppliersButtonCommand { get; }

        private void UpdateSuppliers(object parameter)
        {
            var supplier = ComboBoxUpdateSelectedItem.Tag as Supplier;
            supplier.CompanyName = SuppliersNameText;
            supplier.SupplierAddress = SuppliersAddressText;
            supplier.ContactInfo = SuppliersContactInfoText;
            if (_admin.UpdateSupplier(supplier))
            {
                MessageBox.Show("The supplier is updated");
                updateSupplierView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update the supplier");
            }
        }

        private bool CanCreateSuppliers(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SuppliersNameText) && !string.IsNullOrWhiteSpace(SuppliersContactInfoText) && !string.IsNullOrWhiteSpace(SuppliersAddressText);
        }

        #endregion
    }
}
