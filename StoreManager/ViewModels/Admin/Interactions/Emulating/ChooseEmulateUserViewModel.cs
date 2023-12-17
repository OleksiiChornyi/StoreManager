using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.SQL_static;
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
using StoreManager.ViewModels.Services;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Client;

namespace StoreManager.ViewModels.Admin.Interactions.Emulating
{
    public class ChooseEmulateUserViewModel : ViewModelBase, IInitializable
    {
        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }
        public AdminStoreInteraction _admin;
        public ChooseEmulateUserViewModel(INavigationService navigation)
        {
            Navigation = navigation;

            EmulateUserCommand = new RelayCommand(EmulateUser);
            ButtonBackCommand = new RelayCommand(GoBack);
        }
        public void Initialize(object parameter = null)
        {
            _admin = parameter as AdminStoreInteraction;

            LoadData();
        }
        private void LoadData()
        {
            ComboBoxCurrentItemsSource = new ObservableCollection<ComboBoxItem>();
            ComboBoxCurrentSelectedItem = null;
            DataTable usersTable = _admin.GetDataFromView("UpdateUserView");
            foreach (DataRow row in usersTable.Rows)
            {
                Role userRole;
                Enum.TryParse(row[2].ToString(), out userRole);
                User user = new User() { UserID = int.Parse(row[0].ToString()), UserName = row[1].ToString(), UserRole = userRole };
                if (user.UserID == _admin.user.UserID)
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
        public ICommand EmulateUserCommand { get; }
        public ICommand ButtonBackCommand { get; }

        private void GoBack(object parameter)
        {
            Navigation.GoBack(_admin);
        }
        private void EmulateUser(object parameter)
        {
            User user = ComboBoxCurrentSelectedItem?.Tag as User;
            if (user != null)
            {
                user.PasswordHash = _admin.GetPasswordHash(user);
                new EmulateUserViewModel(user);
                Navigation.GoBack(_admin);
            }
        }
        #endregion
    }
}
