using Microsoft.Win32;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.SQL_static;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Help;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Security.Principal;

namespace StoreManager.ViewModels.StoreInteraction
{
    public class ChangeProfileDataViewModel : ViewModelBase, IBorderDopViewModel, IInitializable
    {
        private AllUsersInteractions _account;
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
        public string FilePath { get; set; }
        public ChangeProfileDataViewModel(INavigationService navigation)
        {
            Navigation = navigation;

            ButtonBackCommand = new RelayCommand(GoBack);
            ChangeDataCommand = new RelayCommand(ChangeProfileDate);
            AddPhotoCommand = new RelayCommand<DragEventArgs>(AddAccountImage);
            BorderDropCommand = new RelayCommand<DragEventArgs>(ExecuteBorderDrop);

        }
        public void Initialize(object parameter = null)
        {
            _account = parameter as AllUsersInteractions;
            LoadData();
        }
        private void LoadData()
        {
            TextAccountName = _account.user.UserName;
            TextEmail = _account.user.Email;
            BirthDate = _account.user.BirthDate;
            TextPhoneNumber = _account.user.PhoneNumber;
            DropBorderText = "Select or drag a file";
            FilePath = null;
        }

        #region Properties
        private string _textAccountName;
        public string TextAccountName
        {
            get => _textAccountName;
            set => SetProperty(ref _textAccountName, value, nameof(TextAccountName));
        }
        private string _textEmail;
        public string TextEmail
        {
            get => _textEmail;
            set => SetProperty(ref _textEmail, value, nameof(TextEmail));
        }
        private string _dropBorderText;
        public string DropBorderText
        {
            get => _dropBorderText;
            set => SetProperty(ref _dropBorderText, value, nameof(DropBorderText));
        }
        private string _textPhoneNumber;
        public string TextPhoneNumber
        {
            get => _textPhoneNumber;
            set
            {
                if (IsTextAllowed(value))
                {
                    SetProperty(ref _textPhoneNumber, value, nameof(TextPhoneNumber));
                }
            }
        }
        private DateTime _birthDate = new DateTime(2020, 1, 1);
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

        #endregion


        #region Commands
        public ICommand BorderDropCommand { get; }
        public ICommand AddPhotoCommand { get; }
        public ICommand ButtonBackCommand { get; }
        public ICommand ChangeDataCommand { get; }
        private void GoBack(object parameter)
        {
            Navigation.GoBack(_account);
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;
            try
            {
                var regex = new Regex("^[+][1-9][0-9]{7,14}$");
                return regex.IsMatch(phoneNumber);
            }
            catch
            {
                return false;
            }
        }
        private void ChangeProfileDate(object parameter)
        {
            if (!IsValidEmail(TextEmail))
            {
                MessageBox.Show("Enter a valid email address");
                return;
            }
            if (!IsValidPhoneNumber(TextPhoneNumber))
            {
                MessageBox.Show("Enter a valid phone number");
                return;
            }
            if (TextAccountName != _account.user.UserName && Checkings.CheckUserNameExistence(TextAccountName))
            {
                MessageBox.Show("This account name is allready exist");
                return;
            }
            if (TextEmail != _account.user.Email && Checkings.CheckUserEmailExistence(TextEmail))
            {
                MessageBox.Show("User with this email is allready exist");
                return;
            }
            if (TextPhoneNumber != _account.user.PhoneNumber && Checkings.CheckUserPhoneNumberExistence(TextPhoneNumber))
            {
                MessageBox.Show("User with this phone number is allready exist");
                return;
            }
            _account.user.UserName = TextAccountName;
            _account.user.Email = TextEmail;
            _account.user.BinaryContent = FilePath == null ? _account.user.BinaryContent : _account.user.BinaryContent == Checkings.standartImage ? _account.CreateBinaryContent(new BinaryContent(FilePath)) : _account.UpdateBinaryContent(new BinaryContent(FilePath) { ContentID = _account.user.BinaryContent.ContentID });
            _account.user.BirthDate = BirthDate;
            _account.user.PhoneNumber = TextPhoneNumber;
            _account.UpdateUserDate();
            Navigation.GoBack(_account);
        }
        private void AddAccountImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Image files (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.png;*.gif;*.bmp",
                InitialDirectory = "c:\\"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length != 1)
                {
                    MessageBox.Show("Select one file");
                    return;
                }

                FilePath = openFileDialog.FileName;
                DropBorderText = System.IO.Path.GetFileName(FilePath);
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
                        string fileExtension = System.IO.Path.GetExtension(filePath);
                        if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp")
                        {
                            FilePath = filePath;
                            DropBorderText = System.IO.Path.GetFileName(filePath);
                        }
                        else
                        {
                            MessageBox.Show("Select an image file");
                        }
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

        #region Helpers
        private bool IsTextAllowed(string text)
        {
            if (text.StartsWith("+"))
            {
                if (text.Equals("+"))
                    return true;
                Regex regex = new Regex("^\\+?[1-9][0-9]{0,14}$");
                return regex.IsMatch(text);
            }
            else
                return false;
        }
        #endregion
    }
}
