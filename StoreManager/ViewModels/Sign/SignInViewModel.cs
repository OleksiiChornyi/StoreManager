using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Admin;
using StoreManager.Models.Client;
using StoreManager.Models.Guest;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Admin;
using StoreManager.ViewModels.Client;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.Sign
{
    public class SignInViewModel : ViewModelBase, IInitializable
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
        private string _textAccountName;
        public string TextAccountName
        {
            get => _textAccountName;
            set => SetProperty(ref _textAccountName, value, nameof(TextAccountName));
        }
        private string _textPassword;
        public string TextPassword
        {
            get => _textPassword;
            set => SetProperty(ref _textPassword, value, nameof(TextPassword));
        }
        public ICommand ButtonBackCommand { get; }
        public ICommand SignInCommand { get; }
        public SignInViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ButtonBackCommand = new RelayCommand(GoBack);
            SignInCommand = new RelayCommand(NavigateToUserViewModel);
        }

        public void Initialize(object parameter = null)
        {
            TextPassword = string.Empty;
        }

        private void GoBack(object parameter)
        {
            Navigation.GoBack();
        }
        private void NavigateToUserViewModel(object parameter)
        {
            if (Checkings.CheckUserNameExistence(TextAccountName))
            {
                AllUsersInteractions user;
                switch (Checkings.GetUserRole(TextAccountName))
                {
                    case Role.client:
                        {
                            user = new StoreForClient(
                                new User(TextAccountName, TextPassword),
                                true);
                            if (user.isOk)
                                Navigation.NavigateTo<ClientViewModel>(user);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                    case Role.guest:
                        {
                            user = new StoreForGuest();
                            if (user.isOk)
                                Navigation.NavigateTo<ClientViewModel>(user);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                    case Role.admin:
                        {
                            user = new StoreManagerForAdmin(new User(TextAccountName, TextPassword));
                            if (user.isOk)
                                Navigation.NavigateTo<AdminViewModel>(user);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("This user does`nt exist");
            }
        }
    }
}
