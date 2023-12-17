using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Admin;
using StoreManager.Models.Client;
using StoreManager.Models.Guest;
using StoreManager.Models.Manager;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Admin;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using StoreManager.ViewModels.StoreInteraction;
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
        public SignInViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ButtonBackCommand = new RelayCommand(GoBack);
            SignInCommand = new RelayCommand(NavigateToUserViewModel);
        }

        public void Initialize(object parameter = null)
        {
            TextAccountName = string.Empty;
            TextPassword = string.Empty;
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

        private void GoBack(object parameter)
        {
            Navigation.GoBack();
        }
        private void NavigateToUserViewModel(object parameter)
        {
            if (Checkings.CheckUserNameExistence(TextAccountName))
            {
                AllUsersInteractions account;
                switch (Checkings.GetUserRole(TextAccountName))
                {
                    case Role.client:
                        {
                            account = new StoreForClient(
                                new User(TextAccountName, TextPassword),
                                true);
                            if (account.user.IsAutorize)
                                Navigation.NavigateTo<MainStoreInterationViewModel>(account);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                    case Role.guest:
                        {
                            account = new StoreForGuest();
                            if (account.user.IsAutorize)
                                Navigation.NavigateTo<MainStoreInterationViewModel>(account);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                    case Role.manager:
                        {
                            account = new StoreManagerForManager(new User(TextAccountName, TextPassword));
                            if (account.user.IsAutorize)
                                Navigation.NavigateTo<MainStoreInterationViewModel>(account);
                            else
                                MessageBox.Show("Error in username or password");
                        }
                        break;
                    case Role.admin:
                        {
                            account = new StoreManagerForAdmin(new User(TextAccountName, TextPassword));
                            if (account.user.IsAutorize)
                                Navigation.NavigateTo<MainStoreInterationViewModel>(account);
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
