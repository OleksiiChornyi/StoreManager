using Microsoft.Win32;
using StoreManager.Models.Abstract.Classes;
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
using System.Windows.Shapes;

namespace StoreManager.Views.AdminPages.Interactions
{
    /// <summary>
    /// Interaction logic for Create_Description.xaml
    /// </summary>
    public partial class Create_Description : Window
    {
        readonly AdminStoreInteraction admin;
        public Create_Description(AdminStoreInteraction admin)
        {
            InitializeComponent();
            this.admin = admin;
        }
        private void CreateDescriptionButton_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void CreateDescriptionButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string filePath = files[0];
                    admin.CreateDescription(filePath);
                    MessageBox.Show("Description created");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Select one file");
                }
            }
        }

        private void CreateDescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = "c:\\"
            };
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileNames.Length > 1 || ofd.FileNames.Length < 1)
                {
                    MessageBox.Show("Select one file");
                    return;
                }
                admin.CreateDescription(ofd.FileName);
                MessageBox.Show("Description created");
                this.Close();
            }
        }
    }
}
