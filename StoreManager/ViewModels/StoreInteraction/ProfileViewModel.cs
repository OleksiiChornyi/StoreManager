using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Data;
using StoreManager.ViewModels.Admin;
using StoreManager.ViewModels.Admin.ViewsInfo;
using StoreManager.ViewModels.CLient;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManager.ViewModels.StoreInteraction
{
    public class ProfileViewModel : ViewModelBase, IInitializable
    {
        private AllUsersInteractions account;
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
        public ProfileViewModel(INavigationService navigation)
        {
            Navigation = navigation;

            ButtonBackCommand = new RelayCommand(GoBack);
            ChangeProfileDataCommand = new RelayCommand(ChangeProfileData);
            OpenAdminViewCommand = new RelayCommand(OpenAdminView);
            ViewOrdersButtonCommand = new RelayCommand(ViewOrdersButton);
            ExitProfileButtonCommand = new RelayCommand(ExitProfileButton);
        }
        public void Initialize(object parameter)
        {
            account = parameter as AllUsersInteractions;
            UserItem = new UserItem(account.user);
            if (account.user.UserRole == Role.admin) //|| Role.manager
            {
                IsVisibleAmninButton = Visibility.Visible;
                AdminButtonContent = "Admin Functions";
            }
            else if (account.user.UserRole == Role.manager)
            {
                IsVisibleAmninButton = Visibility.Visible;
                AdminButtonContent = "Manager Functions";
            }
            else
            {
                IsVisibleAmninButton = Visibility.Collapsed;
            }

            IsEnabledExitButton = !Navigation.isEmulated;

            TextAccountName = account.user.UserName;
            TextEmail = account.user.Email;
            TextRole = account.user.UserRole.ToString();
            TextCountOrders = account.user.OrderCount.ToString();
            TextBirthDate = account.user.BirthDate.ToString();
            TextPhoneNumber = account.user.PhoneNumber;
            TextCreatingDate = account.user.CreatingDate.ToString();
            IsEnabledViewOrdersButton = account.user.OrderCount != 0;
        }
        #region Functions
        #endregion

        #region Properties
        private UserItem _userItem;
        public UserItem UserItem
        {
            get { return _userItem; }
            set
            {
                _userItem = value;
                OnPropertyChanged(nameof(UserItem));
            }
        }
        private Visibility _isVisibleAmninButton;
        public Visibility IsVisibleAmninButton
        {
            get { return _isVisibleAmninButton; }
            set
            {
                _isVisibleAmninButton = value;
                OnPropertyChanged(nameof(IsVisibleAmninButton));
            }
        }
        private bool _isEnabledExitButton;
        public bool IsEnabledExitButton
        {
            get { return _isEnabledExitButton; }
            set
            {
                _isEnabledExitButton = value;
                OnPropertyChanged(nameof(IsEnabledExitButton));
            }
        }
        private string _adminButtonContent;
        public string AdminButtonContent
        {
            get { return _adminButtonContent; }
            set { SetProperty(ref _adminButtonContent, value, nameof(AdminButtonContent)); }
        }
        private string _textAccountName;
        public string TextAccountName
        {
            get { return _textAccountName; }
            set { SetProperty(ref _textAccountName, value, nameof(TextAccountName)); }
        }
        private string _textEmail;
        public string TextEmail
        {
            get { return _textEmail; }
            set { SetProperty(ref _textEmail, value, nameof(TextEmail)); }
        }
        private string _textRole;
        public string TextRole
        {
            get { return _textRole; }
            set { SetProperty(ref _textRole, value, nameof(TextRole)); }
        }
        private string _textCountOrders;
        public string TextCountOrders
        {
            get { return _textCountOrders; }
            set { SetProperty(ref _textCountOrders, value, nameof(TextCountOrders)); }
        }
        private string _textBirthDate;
        public string TextBirthDate
        {
            get { return _textBirthDate; }
            set { SetProperty(ref _textBirthDate, value, nameof(TextBirthDate)); }
        }
        private string _textPhoneNumber;
        public string TextPhoneNumber
        {
            get { return _textPhoneNumber; }
            set { SetProperty(ref _textPhoneNumber, value, nameof(TextPhoneNumber)); }
        }
        private string _textCreatingDate;
        public string TextCreatingDate
        {
            get { return _textCreatingDate; }
            set { SetProperty(ref _textCreatingDate, value, nameof(TextCreatingDate)); }
        }
        private bool _isEnabledViewOrdersButton;
        public bool IsEnabledViewOrdersButton
        {
            get { return _isEnabledViewOrdersButton; }
            set
            {
                _isEnabledViewOrdersButton = value;
                OnPropertyChanged(nameof(IsEnabledViewOrdersButton));
            }
        }
        #endregion

        #region Commands
        public ICommand ButtonBackCommand { get; }
        public ICommand ChangeProfileDataCommand { get; }
        public ICommand OpenAdminViewCommand { get; }
        public ICommand ViewOrdersButtonCommand { get; }
        public ICommand ExitProfileButtonCommand { get; }
        private void GoBack(object parameter)
        {
            Navigation.GoBack(account);
        }
        private void ChangeProfileData(object parameter)
        {
            Navigation.NavigateTo<ChangeProfileDataViewModel>(account);
        }
        private void OpenAdminView(object parameter)
        {
            Navigation.NavigateTo<AdminViewModel>(account);
        }
        private void ViewOrdersButton(object parameter)
        {
            ViewMyOrdersViewModel viewMyOrders = new ViewMyOrdersViewModel(account);
        }
        private void ExitProfileButton(object parameter)
        {
            Navigation.GoBackWhile();
        }
        #endregion
    }
}
