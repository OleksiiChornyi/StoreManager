using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Deleting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.SQL_static;

namespace StoreManager.ViewModels.Admin.Interactions.Deleting
{
    public class DeleteUserViewModel : ViewModelBase
    {
        public AdminStoreInteraction _admin;
        private readonly DeleteUserView deleteUserView;
        public DeleteUserViewModel(AdminStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();

            DeleteItemButtonCommand = new RelayCommand(DeleteItem);

            deleteUserView = new DeleteUserView()
            {
                DataContext = this
            };
            deleteUserView.Show();
        }
        private void LoadData()
        {
            DataTable usersTable = _admin.GetDataFromView("UpdateUserView");
            foreach (DataRow row in usersTable.Rows)
            {
                User user = new User() { UserID = int.Parse(row[0].ToString()), UserName = row[1].ToString() };
                if (user.UserID == _admin.user.UserID || user.UserID == Checkings.guest.UserID)
                    continue;
                ComboBoxCurrentItemsSource.Add(new ComboBoxItem
                {
                    Tag = user,
                    Content = user.UserName
                });
            }
        }
        #region Properties
        private ObservableCollection<ComboBoxItem> _comboBoxCurrentItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCurrentItemsSource
        {
            get => _comboBoxCurrentItemsSource;
            set
            {
                if (_comboBoxCurrentItemsSource != value)
                {
                    _comboBoxCurrentItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxCurrentSelectedItem;
        public ComboBoxItem ComboBoxCurrentSelectedItem
        {
            get => _comboBoxCurrentSelectedItem;
            set
            {
                if (_comboBoxCurrentSelectedItem != value)
                {
                    _comboBoxCurrentSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentSelectedItem));
                }
            }
        }
        #endregion
        #region Commands
        public ICommand DeleteItemButtonCommand { get; }

        private void DeleteItem(object parameter)
        {
            User user = ComboBoxCurrentSelectedItem?.Tag as User;
            if (user != null)
            {
                if (_admin.DeleteUser(user))
                {
                    MessageBox.Show("User deleted");
                    deleteUserView.Close();
                }
                else
                {
                    MessageBox.Show("Can`t delete this user");
                }
            }
        }
        #endregion
    }
}
