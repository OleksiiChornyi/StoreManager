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
    public class CreateSuppliersViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly CreateSuppliersView createSuppliersView;

        public CreateSuppliersViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateSuppliersButtonCommand = new RelayCommand(CreateSuppliers, CanCreateSuppliers);

            createSuppliersView = new CreateSuppliersView()
            {
                DataContext = this
            };
            createSuppliersView.Show();
        }

        #region Properties

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
        public ICommand CreateSuppliersButtonCommand { get; }

        private void CreateSuppliers(object parameter)
        {
            _admin.CreateSupplier(SuppliersNameText, SuppliersContactInfoText, SuppliersAddressText);
            MessageBox.Show("The supplier is created");
            createSuppliersView.Close();
        }

        private bool CanCreateSuppliers(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SuppliersNameText) && !string.IsNullOrWhiteSpace(SuppliersContactInfoText) && !string.IsNullOrWhiteSpace(SuppliersAddressText);
        }

        #endregion
    }
}
