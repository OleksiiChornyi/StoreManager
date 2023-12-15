using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StoreManager.ViewModels.Sign
{
    public class MainSignViewModel : ViewModelBase
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

        public ICommand NavigateToSignInCommand { get; }
        public ICommand NavigateToSignUpCommand { get; }
        public ICommand NavigateToContGuestCommand { get; }
        public MainSignViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            NavigateToSignInCommand = new RelayCommand(NavigateToSignInViewModel);
            NavigateToSignUpCommand = new RelayCommand(NavigateToSignUp);
            NavigateToContGuestCommand = new RelayCommand(GoBackWhile);
        }

        private void NavigateToSignInViewModel(object parameter)
        {
            Navigation.NavigateTo<SignInViewModel>();
        }

        private void NavigateToSignUp(object parameter)
        {
            Navigation.NavigateTo<SignUpViewModel>();
        }
        private void GoBackWhile(object parameter)
        {
            Navigation.GoBackWhile();
        }
    }
}
