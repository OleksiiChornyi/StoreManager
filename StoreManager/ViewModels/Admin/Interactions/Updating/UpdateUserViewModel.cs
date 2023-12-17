using Microsoft.Win32;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Updating;
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
using System.Runtime.Remoting.Contexts;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.SQL_static;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateUserViewModel : ViewModelBase
    {
        private readonly AdminStoreInteraction _admin;
        private readonly UpdateUserView updateUserView;

        public UpdateUserViewModel(AdminStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateUserButtonCommand = new RelayCommand(UpdateUserButton);


            LoadData();

            updateUserView = new UpdateUserView()
            {
                DataContext = this
            };
            updateUserView.Show();
        }
        private void LoadData()
        {
            DataTable usersTable = _admin.GetDataFromView("UpdateUserView");
            foreach (DataRow row in usersTable.Rows)
            {
                Role userRole;
                Enum.TryParse(row[2].ToString(), out userRole);
                User user = new User() { UserID = int.Parse(row[0].ToString()), UserName = row[1].ToString(), UserRole = userRole };
                if (user.UserID == _admin.user.UserID || user.UserID == Checkings.guest.UserID)
                    continue;
                ComboBoxCurrentUserItemsSource.Add(new ComboBoxItem
                {
                    Tag = user,
                    Content = user.UserName
                });
            }
        }
        #region Properties
        private ObservableCollection<ComboBoxItem> _comboBoxCurrentUsertemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCurrentUserItemsSource
        {
            get => _comboBoxCurrentUsertemsSource;
            set
            {
                if (_comboBoxCurrentUsertemsSource != value)
                {
                    _comboBoxCurrentUsertemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentUserItemsSource));
                }
            }
        }
        private ComboBoxItem _comboBoxCurrentUserSelectedItem;
        public ComboBoxItem ComboBoxCurrentUserSelectedItem
        {
            get => _comboBoxCurrentUserSelectedItem;
            set
            {
                if (_comboBoxCurrentUserSelectedItem != value)
                {
                    User user = value.Tag as User;
                    ComboBoxItem selectItem = ComboBoxUserRoleItemsSource
                        .FirstOrDefault(item => item.Content as string == user.UserRole.ToString());
                    ComboBoxUserRoleSelectedItem = selectItem;

                    _comboBoxCurrentUserSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentUserSelectedItem));
                }
            }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxUserRoleItemsSource = new ObservableCollection<ComboBoxItem>(new List<ComboBoxItem>() 
        { 
            new ComboBoxItem() { Content = Role.admin.ToString() },
            new ComboBoxItem() { Content = Role.client.ToString() },
            new ComboBoxItem() { Content = Role.manager.ToString() }
        });
        public ObservableCollection<ComboBoxItem> ComboBoxUserRoleItemsSource
        {
            get => _comboBoxUserRoleItemsSource;
            set
            {
                if (_comboBoxUserRoleItemsSource != value)
                {
                    _comboBoxUserRoleItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxUserRoleItemsSource));
                }
            }
        }
        private ComboBoxItem _comboBoxUserRoleSelectedItem;
        public ComboBoxItem ComboBoxUserRoleSelectedItem
        {
            get => _comboBoxUserRoleSelectedItem;
            set
            {
                if (_comboBoxUserRoleSelectedItem != value)
                {
                    _comboBoxUserRoleSelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxUserRoleSelectedItem));
                }
            }
        }
        #endregion

        #region Commands

        public ICommand UpdateUserButtonCommand { get; }

        private void UpdateUserButton(object parameter)
        {
            User user = ComboBoxCurrentUserSelectedItem.Tag as User;

            Role userRole;
            Enum.TryParse(ComboBoxUserRoleSelectedItem.Content.ToString(), out userRole);

            user.UserRole = userRole;

            if (_admin.UpdateUserRole(user))
            {
                MessageBox.Show("User updated");
                updateUserView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update this user");
            }
        }

        #endregion
    }
}
