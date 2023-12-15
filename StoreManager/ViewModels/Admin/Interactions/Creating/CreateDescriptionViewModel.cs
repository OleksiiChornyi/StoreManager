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
using Microsoft.Win32;
using StoreManager.ViewModels.Help;

namespace StoreManager.ViewModels.Admin.Interactions.Creating
{
    public class CreateDescriptionViewModel : ViewModelBase, IBorderDopViewModel
    {
        private readonly AdminStoreInteraction _admin;
        private readonly CreateDescriptionView createDescriptionView;

        public CreateDescriptionViewModel(AdminStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            CreateDescriptionButtonCommand = new RelayCommand(CreateDescription);
            BorderDropCommand = new RelayCommand<DragEventArgs>(ExecuteBorderDrop);

            DropBorderText = "Select or drag a file to describe";

            createDescriptionView = new CreateDescriptionView()
            {
                DataContext = this
            };
            createDescriptionView.Show();
        }

        #region Properties
        private string _dropBorderText;
        public string DropBorderText
        {
            get => _dropBorderText;
            set => SetProperty(ref _dropBorderText, value, nameof(DropBorderText));
        }
        #endregion

        #region Commands

        public ICommand CreateDescriptionButtonCommand { get; }
        public ICommand BorderDropCommand { get; }

        private void CreateDescription(object parameter)
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
                _admin.CreateDescription(ofd.FileName);
                MessageBox.Show("Description created");
                createDescriptionView.Close();
            }
        }

        public void ExecuteBorderDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1)
                {
                    string filePath = files[0];

                    if (System.IO.File.Exists(filePath))
                    {
                        _admin.CreateDescription(filePath);
                        MessageBox.Show("Description created");
                        createDescriptionView.Close();
                    }
                    else
                    {
                        MessageBox.Show("Select one file");
                    }
                }
                else
                {
                    MessageBox.Show("Select one file");
                }
            }
        }

        #endregion
    }
}
