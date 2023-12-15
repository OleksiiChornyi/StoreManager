using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Admin.Interactions;
using StoreManager.ViewModels.Admin.Interactions.Creating;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using StoreManager.ViewModels.StoreInteraction;
using StoreManager.Views.Admin.Interactions;
using StoreManager.Views.StoreInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreManager.ViewModels.Admin
{
    public class AdminViewModel : ViewModelBase, IInitializable
    {
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
        public AdminStoreInteraction admin;
        public ICommand ExitCommand { get; }
        #region Create commands
        public ICommand CreateCategoryCommand { get; }
        public ICommand CreateDescriptionCommand { get; }
        public ICommand CreateProductCommand { get; }
        public ICommand CreateSuppliersCommand { get; }
        public ICommand CreateShipmentsCommand { get; }
        public ICommand CreateWarehousesCommand { get; }
        public ICommand CreateInventoryCommand { get; }
        #endregion
        #region Update commands
        public ICommand UpdateProductsCommand { get; }

        #endregion
        public AdminViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ExitCommand = new RelayCommand(GoBackWhile);
            #region Create commands
            CreateCategoryCommand = new RelayCommand(ShowCreateCategory);
            CreateDescriptionCommand = new RelayCommand(ShowCreateDescription);
            CreateProductCommand = new RelayCommand(ShowCreateProduct);
            CreateSuppliersCommand = new RelayCommand(ShowCreateSuppliers);
            CreateShipmentsCommand = new RelayCommand(ShowCreateShipments);
            CreateWarehousesCommand = new RelayCommand(ShowCreateWarehouses);
            CreateInventoryCommand = new RelayCommand(ShowCreateInventory);
            #endregion
            #region Update commands
            UpdateProductsCommand = new RelayCommand(ShowUpdateProducts);

            #endregion
        }
        public void Initialize(object parameter)
        {
            admin = parameter as AdminStoreInteraction;
        }

        private void GoBackWhile(object parameter)
        {
            Navigation.GoBackWhile();
        }
        #region Create commands
        private void ShowCreateCategory(object parameter)
        {
            new CreateCategoryViewModel(admin);
        }
        
        private void ShowCreateDescription(object parameter)
        {
            new CreateDescriptionViewModel(admin);
        }
        private void ShowCreateProduct(object parameter)
        {
            new CreateProductViewModel(admin);
        }
        private void ShowCreateSuppliers(object parameter)
        {
            new CreateSuppliersViewModel(admin);
        }
        private void ShowCreateShipments(object parameter)
        {
            new CreateShipmentsViewModel(admin);
        }
        private void ShowCreateWarehouses(object parameter)
        {
            new CreateWarehousesViewModel(admin);
        }
        private void ShowCreateInventory(object parameter)
        {
            new CreateInventoryViewModel(admin);
        }
        #endregion
        #region Update commands
        private void ShowUpdateProducts(object parameter)
        {
            new MainStoreInterationViewModel(account: admin);
        }
        #endregion
    }
}
