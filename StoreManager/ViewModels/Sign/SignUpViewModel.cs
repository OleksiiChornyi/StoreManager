using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Client;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Client;
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
    public class SignUpViewModel : ViewModelBase, IInitializable
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
        private string _textEmail;
        public string TextEmail
        {
            get => _textEmail;
            set => SetProperty(ref _textEmail, value, nameof(TextEmail));
        }
        private string _textPhoneNumber;
        public string TextPhoneNumber
        {
            get => _textPhoneNumber;
            set => SetProperty(ref _textPhoneNumber, value, nameof(TextPhoneNumber));
        }
        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }
        private string _textFilePath;
        public string TextFilePath
        {
            get => _textFilePath;
            set => SetProperty(ref _textFilePath, value, nameof(TextFilePath));
        }
        public ICommand ButtonBackCommand { get; }
        public ICommand SignUpCommand { get; }
        public SignUpViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ButtonBackCommand = new RelayCommand(GoBack);
            SignUpCommand = new RelayCommand(NavigateToClientViewModel);
        }

        public void Initialize(object parameter = null)
        {
            TextPassword = string.Empty;
        }
        private void GoBack(object parameter)
        {
            Navigation.GoBack();
        }
        private void NavigateToClientViewModel(object parameter)
        {
            if (Checkings.CheckUserExistence(TextAccountName, TextEmail, TextPhoneNumber))
            {
                MessageBox.Show("This user already exist");
            }
            else
            {
                Navigation.NavigateTo<ClientViewModel>(new StoreForClient(
                                new User(TextAccountName, TextPassword, TextEmail, new BinaryContent(), BirthDate, TextPhoneNumber),
                                false));
            }
        }
    }
}
