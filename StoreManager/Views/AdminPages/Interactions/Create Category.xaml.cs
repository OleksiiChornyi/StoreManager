using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace StoreManager.Views.AdminPages.Interactions
{
    /// <summary>
    /// Interaction logic for Create_Category.xaml
    /// </summary>
    public partial class Create_Category : Window
    {
        readonly AdminStoreInteraction admin;
        public Create_Category(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }

        private void CreateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            int? parentCategoryId = null;
            if (comboBoxParentCategory.SelectedItem is ComboBoxItem selectedCategory)
                parentCategoryId = int.Parse(selectedCategory.ToolTip.ToString());
            admin.CreateCategory(CategoryNameTextBox.Text, CategoryDescriptionTextBox.Text, parentCategoryId);
            MessageBox.Show("Category created");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable categoriesTable = admin.GetDataFromView("ProductsCategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = row[1],
                    ToolTip = row[0]
                };
                comboBoxParentCategory.Items.Add(item);
            }
        }
    }
}
