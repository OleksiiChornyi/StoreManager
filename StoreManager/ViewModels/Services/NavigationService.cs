using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.StoreInteraction;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreManager.ViewModels.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModelBase _currentView;
        private readonly Func<Type, ViewModelBase> _viewModelFactory;
        private readonly Stack<ViewModelBase> _navigationStack;
        public bool isEmulated { get; set; } = false;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory, bool isEmulated)
        {
            _viewModelFactory = viewModelFactory;
            _navigationStack = new Stack<ViewModelBase>();
            this.isEmulated = isEmulated;
        }

        public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
        {
            ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            _navigationStack.Push(CurrentView);
            CurrentView = viewModel;

            if (viewModel is IInitializable initializable)
            {
                initializable.Initialize(parameter);
            }
        }

        public void GoBack(object parameter = null)
        {
            if (_navigationStack.Count > 0)
            {
                ViewModelBase previousView = _navigationStack.Pop();
                CurrentView = previousView;
                if (CurrentView is IInitializable initializable)
                {
                    initializable.Initialize(parameter);
                }
            }
        }

        public void GoBackWhile()
        {
            while (_navigationStack.Count > 0)
            {
                _navigationStack.Pop();
            }
            NavigateTo<MainStoreInterationViewModel>();
        }
    }
}
