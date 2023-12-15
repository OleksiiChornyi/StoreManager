using StoreManager.ViewModels.Core;
using System;
using System.Collections.Generic;
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

        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _navigationStack = new Stack<ViewModelBase>();
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

        public void GoBack()
        {
            if (_navigationStack.Count > 0)
            {
                ViewModelBase previousView = _navigationStack.Pop();
                CurrentView = previousView;
            }
        }

        public void GoBackWhile()
        {
            while (_navigationStack.Count > 2)
            {
                _navigationStack.Pop();
            }
            if (_navigationStack.Count > 0)
            {
                CurrentView = _navigationStack.Pop();
                _navigationStack.Push(CurrentView);
            }
        }
        /*public void OpenWindow<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
        {
            ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModel));

            if (viewModel is IInitializable initializable)
            {
                initializable.Initialize(parameter);
            }

            Window newWindow = new Window
            {
                DataContext = viewModel,
                Content = viewModel
            };

            newWindow.Show();
        }*/
    }
}
