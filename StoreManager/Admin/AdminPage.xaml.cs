using StoreManager.Abstract.Classes;
using StoreManager.Abstract.Interfaces;
using StoreManager.Admin.Interactions;
using StoreManager.Admin.ViewsInfo;
using StoreManager.Client;
using StoreManager.SignPages;
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

namespace StoreManager.Admin
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private AdminStoreInteraction account;
        public AdminPage(string UserName, string password, string contactInfo = "")
        {
            InitializeComponent();
            account = new StoreManagerForAdmin(UserName, password, contactInfo);
            if (account == null || !account.isOk)
            {
                if (account.isExist)
                {
                    MessageBox.Show("Користувач з таким ім'ям вже існує!");
                }
                else
                {
                    MessageBox.Show("Виникла якась помилка!\nСпробуйте ще раз");
                }
            }
        }


        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainSignPage());
        }

        public bool GetAutorization()
        {
            if (account == null)
                return false;
            return account.isOk;
        }

        private void CreateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Category createCategoryForm = new Create_Category(account);
            createCategoryForm.Show();
        }

        private void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {
            Create_product createProductForm = new Create_product(account);
            createProductForm.Show();
        }

        private void CreateDescritpinButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Description createDescriptionForm = new Create_Description(account);
            createDescriptionForm.Show();
        }

        private void CreateSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Suppliers createSuppliersForm = new Create_Suppliers(account);
            createSuppliersForm.Show();
        }

        private void CreateShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Shipments createShipmentsForm = new Create_Shipments(account);
            createShipmentsForm.Show();
        }

        private void CreateWarehousesButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Warehouses createWarehousesForm = new Create_Warehouses(account);
            createWarehousesForm.Show();
        }

        private void CreateInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            Create_Inventory createInventoryForm = new Create_Inventory(account);
            createInventoryForm.Show();
        }

        private void ViewCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ViewCategories viewCategoriesForm = new ViewCategories(account);
            viewCategoriesForm.Show();
        }

        private void ViewDescritpinsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewDescription viewDescriptionForm = new ViewDescription(account);
            viewDescriptionForm.Show();
        }

        private void ViewProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewProducts viewProductsForm = new ViewProducts(account);
            viewProductsForm.Show();
        }

        private void ViewSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewSuppliers viewSuppliersForm = new ViewSuppliers(account);
            viewSuppliersForm.Show();
        }

        private void ViewShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewShipments viewShipmentsForm = new ViewShipments(account);
            viewShipmentsForm.Show();
        }

        private void ViewWarehousesButton_Click(object sender, RoutedEventArgs e)
        {
            ViewWarehouses viewWarehousesForm = new ViewWarehouses(account);
            viewWarehousesForm.Show();
        }

        private void ViewInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            ViewInventory viewInventoryForm = new ViewInventory(account);
            viewInventoryForm.Show();
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewOrders viewOrdersForm = new ViewOrders(account);
            viewOrdersForm.Show();
        }

        private void ViewOrderItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewOrderItems viewOrderItemsForm = new ViewOrderItems(account);
            viewOrderItemsForm.Show();
        }

        private void ViewUsersItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUsers viewUsersForm = new ViewUsers(account);
            viewUsersForm.Show();
        }
    }
}
