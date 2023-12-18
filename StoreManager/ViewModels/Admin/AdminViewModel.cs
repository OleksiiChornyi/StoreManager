using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.ViewModels.Admin.Interactions;
using StoreManager.ViewModels.Admin.Interactions.Creating;
using StoreManager.ViewModels.Admin.Interactions.Deleting;
using StoreManager.ViewModels.Admin.Interactions.Emulating;
using StoreManager.ViewModels.Admin.Interactions.Updating;
using StoreManager.ViewModels.Admin.ViewsInfo;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.DB_Data;
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
using System.Windows;
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
        public ManagerStoreInteraction admin;
        public AdminViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ButtonBackCommand = new RelayCommand(GoBack);
            #region Category commands
            CreateCategoryCommand = new RelayCommand(ShowCreateCategory);
            UpdateCategoryCommand = new RelayCommand(ShowUpdateCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            ViewCategoriesCommand = new RelayCommand(ShowViewCategories);
            #endregion
            #region Description cammands
            CreateDescriptionCommand = new RelayCommand(ShowCreateDescription);
            UpdateDescriptionCommand = new RelayCommand(ShowUpdateDescription);
            DeleteDescriptionCommand = new RelayCommand(DeleteDescription);
            ViewDescriptionsCommand = new RelayCommand(ShowViewDescriptions);
            #endregion
            #region Product cammands
            CreateProductCommand = new RelayCommand(ShowCreateProduct);
            UpdateProductCommand = new RelayCommand(ShowUpdateProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
            ViewProductsCommand = new RelayCommand(ShowViewProducts);
            #endregion
            #region User cammands
            UpdateUserCommand = new RelayCommand(ShowUpdateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            ViewUsersCommand = new RelayCommand(ViewUsers);
            EmulateUserCommand = new RelayCommand(EmulateUser);
            #endregion
            #region Orders commands
            UpdateOrdersCommand = new RelayCommand(UpdateOrders);
            DeleteOrdersCommand = new RelayCommand(DeleteOrders);
            ViewOrdersCommand = new RelayCommand(ViewOrders);
            #endregion
            #region Suppliers cammands
            CreateSuppliersCommand = new RelayCommand(CreateSupplier);
            UpdateSuppliersCommand = new RelayCommand(UpdateSupplier);
            ViewSuppliersrCommand = new RelayCommand(ViewSuppliers);
            DeleteSuppliersCommand = new RelayCommand(DeleteSuppliers);
            #endregion
            #region Inventories cammands
            CreateInventoryCommand = new RelayCommand(CreateInventory);
            UpdateInventoryCommand = new RelayCommand(UpdateInventory);
            DeleteInventoryCommand = new RelayCommand(DeleteInventory);
            ViewInventoryCommand = new RelayCommand(ViewInventory);
            #endregion
            #region Warehouses cammands
            CreateWarehouseCommand = new RelayCommand(CreateWarehouse);
            UpdateWarehouseCommand = new RelayCommand(UpdateWarehouse);
            DeleteWarehouseCommand = new RelayCommand(DeleteWarehouse);
            ViewWarehouseCommand = new RelayCommand(ViewWarehouse);
            #endregion
            #region Audit Logs Command
            ViewAuditLogsCommand = new RelayCommand(ViewAuditLogs);
            #endregion
            #region Select Data of DB
            SelectTablesCommand = new RelayCommand(SelectTables);
            SelectTProceduresCommand = new RelayCommand(SelectTProcedures);
            SelectFunctionsCommand = new RelayCommand(SelectFunctions);
            SelectTriggersCommand = new RelayCommand(SelectTriggers);
            SelectSequencesCommand = new RelayCommand(SelectSequences);
            SelectViewsCommand = new RelayCommand(SelectViews);
            #endregion
        }
        public void Initialize(object parameter)
        {
            admin = parameter as ManagerStoreInteraction;
            switch (admin.user.UserRole)
            {
                case Role.admin:
                    IsVisibleForManager = Visibility.Visible;
                    break;
                case Role.manager:
                    IsVisibleForManager = Visibility.Collapsed;
                    break;
                default:
                    IsVisibleForManager = Visibility.Collapsed;
                    break;
            }
        }
        #region Properties
        private Visibility _isVisibleForManager;
        public Visibility IsVisibleForManager
        {
            get => _isVisibleForManager;
            set
            {
                _isVisibleForManager = value;
                OnPropertyChanged(nameof(IsVisibleForManager));
            }
        }
        #endregion

        #region Category commands
        public ICommand CreateCategoryCommand { get; }
        public ICommand UpdateCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand ViewCategoriesCommand { get; }
        private void ShowCreateCategory(object parameter)
        {
            new CreateCategoryViewModel(admin);
        }
        private void ShowUpdateCategory(object parameter)
        {
            new UpdateCategoryViewModel(admin);
        }
        private void DeleteCategory(object parameter)
        {
            new DeleteCategoryViewModel(admin);
        }
        private void ShowViewCategories(object parameter)
        {
            new ViewCategoriesViewModel(admin);
        }
        #endregion
        #region Descriptions commands
        public ICommand CreateDescriptionCommand { get; }
        public ICommand UpdateDescriptionCommand { get; }
        public ICommand DeleteDescriptionCommand { get; }
        public ICommand ViewDescriptionsCommand { get; }
        private void ShowCreateDescription(object parameter)
        {
            new CreateDescriptionViewModel(admin);
        }
        private void ShowUpdateDescription(object parameter)
        {
            new UpdateDescriptionViewModel(admin);
        }
        private void DeleteDescription(object parameter)
        {
            new DeleteDescriptionViewModel(admin);
        }
        private void ShowViewDescriptions(object parameter)
        {
           new ViewDescriptionViewModel(admin);
        }
        #endregion
        #region Product cammands
        public ICommand CreateProductCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ViewProductsCommand { get; }
        private void ShowCreateProduct(object parameter)
        {
            new CreateProductViewModel(admin);
        }
        private void ShowUpdateProduct(object parameter)
        {
            new UpdateProductViewModel(admin);
        }
        private void DeleteProduct(object parameter)
        {
            new DeleteProductViewModel(admin as AdminStoreInteraction);
        }
        private void ShowViewProducts(object parameter)
        {
            new ViewProductViewModel(admin);
        }
        #endregion
        #region User cammands
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ViewUsersCommand { get; }
        public ICommand EmulateUserCommand { get; }
        private void ShowUpdateUser(object parameter)
        {
            new UpdateUserViewModel(admin as AdminStoreInteraction);
        }
        private void DeleteUser(object parameter)
        {
            new DeleteUserViewModel(admin as AdminStoreInteraction);
        }
        private void ViewUsers(object parameter)
        {
            new ViewUsersViewModel(admin);
        }
        private void EmulateUser(object parameter)
        {
            Navigation.NavigateTo<ChooseEmulateUserViewModel>(admin as AdminStoreInteraction);
        }
        #endregion
        #region Orders cammands
        public ICommand UpdateOrdersCommand { get; }
        public ICommand DeleteOrdersCommand { get; }
        public ICommand ViewOrdersCommand { get; }
        private void UpdateOrders(object parameter)
        {
            new UpdateOrdersViewModel(admin);
        }
        private void DeleteOrders(object parameter)
        {
            new DeleteOrdersViewModel(admin as AdminStoreInteraction);
        }
        private void ViewOrders(object parameter)
        {
            new ViewOrdersViewModel(admin);
        }
        #endregion
        #region Suppliesr cammands
        public ICommand CreateSuppliersCommand { get; }
        public ICommand UpdateSuppliersCommand { get; }
        public ICommand ViewSuppliersrCommand { get; }
        public ICommand DeleteSuppliersCommand { get; }
        private void CreateSupplier(object parameter)
        {
            new CreateSuppliersViewModel(admin);
        }
        private void UpdateSupplier(object parameter)
        {
            new UpdateSupplierViewModel(admin);
        }
        private void ViewSuppliers(object parameter)
        {
            new ViewSupplierViewModel(admin);
        }
        private void DeleteSuppliers(object parameter)
        {
            new DeleteSuppliersViewModel(admin);
        }
        #endregion
        #region Inventories cammands
        public ICommand CreateInventoryCommand { get; }
        public ICommand UpdateInventoryCommand { get; }
        public ICommand DeleteInventoryCommand { get; }
        public ICommand ViewInventoryCommand { get; }
        private void CreateInventory(object parameter)
        {
            new CreateInventoryViewModel(admin);
        }
        private void UpdateInventory(object parameter)
        {
            new UpdateInventoryViewModel(admin);
        }
        private void DeleteInventory(object parameter)
        {
            new DeleteInventoryViewModel(admin);
        }
        private void ViewInventory(object parameter)
        {
            new ViewInventoryViewModel(admin);
        }
        #endregion
        #region Warehouses cammands
        public ICommand CreateWarehouseCommand { get; }
        public ICommand UpdateWarehouseCommand { get; }
        public ICommand DeleteWarehouseCommand { get; }
        public ICommand ViewWarehouseCommand { get; }
        private void CreateWarehouse(object parameter)
        {
            new CreateWarehousesViewModel(admin);
        }
        private void UpdateWarehouse(object parameter)
        {
            new UpdateWarehousesViewModel(admin);
        }
        private void DeleteWarehouse(object parameter)
        {
            new DeleteWarehouseViewModel(admin);
        }
        private void ViewWarehouse(object parameter)
        {
            new ViewWarehouseViewModel(admin);
        }
        #endregion
        #region Audit Logs Commnd
        public ICommand ViewAuditLogsCommand { get; }
        private void ViewAuditLogs(object parameter)
        {
            new ViewAuditLogssViewModel(admin);
        }
        #endregion
        #region Select Data of DB
        public ICommand SelectTablesCommand { get; }
        public ICommand SelectTProceduresCommand { get; }
        public ICommand SelectFunctionsCommand { get; }
        public ICommand SelectTriggersCommand { get; }
        public ICommand SelectSequencesCommand { get; }
        public ICommand SelectViewsCommand { get; }
        private void SelectTables(object parameter)
        {
            new TablesDBViewModel(admin);
        }
        private void SelectTProcedures(object parameter)
        {
            new ProceduresDBViewModel(admin);
        }
        private void SelectFunctions(object parameter)
        {
            new FunctionsDBViewModel(admin);
        }
        private void SelectTriggers(object parameter)
        {
            new TriggersDBViewModel(admin);
        }
        private void SelectSequences(object parameter)
        {
            new SequencesDBViewModel(admin);
        }
        private void SelectViews(object parameter)
        {
            new ViewsDBViewModel(admin);
        }
        #endregion
        public ICommand ButtonBackCommand { get; }

        private void GoBack(object parameter)
        {
            Navigation.GoBack(admin);
        }
    }
}
