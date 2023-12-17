using Microsoft.Win32;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Help;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data;
using StoreManager.Views.Admin.Interactions.Updating;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateDescriptionViewModel : ViewModelBase, IBorderDopViewModel
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateDescriptionView updateDescriptionView;

        public UpdateDescriptionViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateDescriptionButtonCommand = new RelayCommand(UpdateDescription);
            BorderDropCommand = new RelayCommand<DragEventArgs>(ExecuteBorderDrop);

            DropBorderText = "Select or drag a file to describe";

            LoadData();

            updateDescriptionView = new UpdateDescriptionView()
            {
                DataContext = this
            };
            updateDescriptionView.Show();
        }
        private void LoadData()
        {
            DataTable categoriesTable = _admin.GetDataFromView("UpdateDescriptionView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                ComboBoxCurrentDescriptionItemsSource.Add(new ComboBoxItem
                {
                    Tag = row[0],
                    Content = row[1]
                });
            }
        }
        #region Properties
        private string _dropBorderText;
        public string DropBorderText
        {
            get => _dropBorderText;
            set => SetProperty(ref _dropBorderText, value, nameof(DropBorderText));
        }
        private bool _updateDescriptionButtonIsEnabled = false;
        public bool UpdateDescriptionButtonIsEnabled
        {
            get => _updateDescriptionButtonIsEnabled;
            set
            {
                if (_updateDescriptionButtonIsEnabled != value)
                {
                    _updateDescriptionButtonIsEnabled = value;
                    OnPropertyChanged(nameof(UpdateDescriptionButtonIsEnabled));
                }
            }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxCurrentDescriptionItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCurrentDescriptionItemsSource
        {
            get => _comboBoxCurrentDescriptionItemsSource;
            set
            {
                if (_comboBoxCurrentDescriptionItemsSource != value)
                {
                    _comboBoxCurrentDescriptionItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentDescriptionItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxCurrentDescriptionSelectedItem;
        public ComboBoxItem ComboBoxCurrentDescriptionSelectedItem
        {
            get => _comboBoxCurrentDescriptionSelectedItem;
            set
            {
                if (_comboBoxCurrentDescriptionSelectedItem != value)
                {
                    _comboBoxCurrentDescriptionSelectedItem = value;
                    UpdateDescriptionButtonIsEnabled = true;
                    OnPropertyChanged(nameof(ComboBoxCurrentDescriptionSelectedItem));
                }
            }
        }
        #endregion

        #region Commands

        public ICommand UpdateDescriptionButtonCommand { get; }
        public ICommand BorderDropCommand { get; }

        private void UpdateDescription(object parameter)
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
                string filePath = ofd.FileName;
                
                int descriptionID = Convert.ToInt32(ComboBoxCurrentDescriptionSelectedItem.Tag);

                Description description = new Description(descriptionID, filePath, "Some Info");

                if (_admin.UpdateDescription(description))
                {
                    MessageBox.Show("Description updated");
                    updateDescriptionView.Close();
                }
                else
                {
                    MessageBox.Show("Can`t update this description");
                }
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

                    if (File.Exists(filePath))
                    {
                        if (ComboBoxCurrentDescriptionSelectedItem == null)
                        {
                            MessageBox.Show("Select a description to update");
                            return;
                        }
                        int descriptionID = Convert.ToInt32(ComboBoxCurrentDescriptionSelectedItem.Tag);

                        Description description = new Description(descriptionID, filePath, "Some Info");

                        _admin.UpdateDescription(description);
                        MessageBox.Show("Description updated");
                        updateDescriptionView.Close();
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
