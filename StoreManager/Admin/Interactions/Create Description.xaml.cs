using Microsoft.Win32;
using StoreManager.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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

namespace StoreManager.Admin.Interactions
{
    /// <summary>
    /// Interaction logic for Create_Description.xaml
    /// </summary>
    public partial class Create_Description : Window
    {
        AdminStoreInteraction admin;

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
                    MessageBox.Show("Опис створений");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Оберіть один файл");
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
                    MessageBox.Show("Оберіть один файл");
                    return;
                }
                admin.CreateDescription(ofd.FileName);
                MessageBox.Show("Опис створений");
                this.Close();
            }
        }
    }
    public class CreateDescriptionButtonIsEnebledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (item is string val && string.IsNullOrEmpty(val.Replace(" ", "")))
                    return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
