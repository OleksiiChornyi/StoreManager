using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Client;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreManager.ViewModels.Client
{
    public class ClientViewModel : ViewModelBase, IInitializable
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
        public UserStoreInteraction client;
        public ICommand ExitCommand { get; set; }
        public ClientViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            ExitCommand = new RelayCommand(GoBackWhile);
        }
        public void Initialize(object parameter)
        {
            client = parameter as UserStoreInteraction;
        }
        private void GoBackWhile(object parameter)
        {
            Navigation.GoBackWhile();
        }
    }
}
