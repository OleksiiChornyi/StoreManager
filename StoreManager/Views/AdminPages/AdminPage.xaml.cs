using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Admin;
using StoreManager.Views.AdminPages.Interactions;
using StoreManager.Views.AdminPages.ViewsInfo;
using StoreManager.Views.SignPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreManager.Views.AdminPages
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly AdminStoreInteraction account;
        public AdminPage(string UserName, string password, string contactInfo = "")
        {
            InitializeComponent();
            account = new StoreManagerForAdmin(UserName, password, contactInfo);
            if (account == null || !account.isOk)
            {
                if (account.isExist)
                {
                    MessageBox.Show("A user with this name already exists!");
                }
                else
                {
                    MessageBox.Show("There was an error!\nPlease try again");
                }
            }
        }

        public bool GetAutorization()
        {
            if (account == null)
                return false;
            return account.isOk;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainSignPage());
        }

        private void CreateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Category createCategoryForm = new Create_Category(account);
            createCategoryForm.Show();
        }

        private void CreateDescritpinButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Description create_Description = new Create_Description(account);
            create_Description.Show();
        }

        private void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Product create_Product = new Create_Product(account);
            create_Product.Show();
        }

        private void CreateSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Suppliers create_Suppliers = new Create_Suppliers(account);
            create_Suppliers.Show();
        }

        private void CreateShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Shipments create_Shipments = new Create_Shipments(account);
            create_Shipments.Show();
        }

        private void CreateWarehousesButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Warehouses create_Warehouses = new Create_Warehouses(account);
            create_Warehouses.Show();
        }

        private void CreateInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Inventory create_Inventory = new Create_Inventory(account);
            create_Inventory.Show();
        }

        private void ViewCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ViewCategories viewCategories = new ViewCategories(account);
            viewCategories.Show();
        }

        private void ViewDescritpinsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewDescription viewDescription = new ViewDescription(account);
            viewDescription.Show();
        }

        private void ViewProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewProducts viewProducts = new ViewProducts(account);
            viewProducts.Show();
        }

        private void ViewSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewSuppliers viewSuppliers = new ViewSuppliers(account);
            viewSuppliers.Show();
        }

        private void ViewShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewShipments viewShipments = new ViewShipments(account);
            viewShipments.Show();
        }

        private void ViewWarehousesButton_Click(object sender, RoutedEventArgs e)
        {
            ViewWarehouses viewWarehouses = new ViewWarehouses(account);
            viewWarehouses.Show();
        }

        private void ViewInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            ViewInventory viewInventory = new ViewInventory(account);
            viewInventory.Show();
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewOrders viewOrders = new ViewOrders(account);
            viewOrders.Show();
        }

        private void ViewOrderItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewOrderItems viewOrderItems = new ViewOrderItems(account);
            viewOrderItems.Show();
        }

        private void ViewUsersItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUsers viewUsers = new ViewUsers(account);
            viewUsers.Show();
        }

        private void UpdateProductsButton_Click(object sender, RoutedEventArgs e)
        {
            Update_Products update_Products = new Update_Products(account);
            update_Products.Show();
        }

        private void ViewPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewPayments viewPayments = new ViewPayments(account);
            viewPayments.Show();
        }

        private void EmulateGuest_Click(object sender, RoutedEventArgs e)
        {
            ClientStoreInteraction.CreateGuestIfNotExist();
            EmulateGuest emulateGuest = new EmulateGuest();
            emulateGuest.Show();
        }
    }
}
