using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Client.Cart;
using StoreManager.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace StoreManager.Views.ClientPages
{
    /// <summary>
    /// Interaction logic for ClientDescriptionView.xaml
    /// </summary>
    public partial class ClientDescriptionView : Window
    {
        readonly ClientStoreInteraction client;
        readonly StoreCartInteraction myCart;
        readonly ImageItem data;
        public ClientDescriptionView(ClientStoreInteraction client, StoreCartInteraction myCart, ImageItem imageItem)
        {
            InitializeComponent();
            this.client = client;
            this.myCart = myCart;
            data = imageItem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (client.myRole == Role.guest)
            {
                DescriptionButton.IsEnabled = false;
                AddOrderItem.IsEnabled = false;
            }
            else
            {
                DescriptionButton.IsEnabled = true;
                AddOrderItem.IsEnabled = true;
            }
            if (data.descriptionId == null)
            {
                DescriptionButton.IsEnabled = false;
                DescriptionButtonTextBlock.Text = "No detailed information available";
            }
            CategoryNameTextBlock.Text = data.categoryName;
            CategoryDescriptionTextBlock.Text = data.categoryDescription;
            ProductNameTextBlock.Text = data.Name;
            ProductPriceTextBlock.Text = data.price.ToString();
            ProductImage.Source = data.Image_src;
        }

        private void DescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (data.descriptionId == null)
                return;
            string fileName = client.GetDescriptionName((int)data.descriptionId);
            if (string.IsNullOrEmpty(fileName))
                return;
            try
            {
                if (!Directory.Exists("tmp"))
                {
                    Directory.CreateDirectory("tmp");
                }
                fileName = "tmp\\" + fileName;

                byte[] fileData = client.GetDecroptionData((int)data.descriptionId);
                client.SaveFileToDisk(fileName, fileData);
                client.OpenFileWithDefaultApplication(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void AddOrderItem_Click(object sender, RoutedEventArgs e)
        {
            myCart.AddOrUpdateItem(data.productId, data.Name, data.price, data.categoryName, 1);
            MessageBox.Show("Product added");
        }
    }
}
