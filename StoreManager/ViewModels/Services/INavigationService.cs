using StoreManager.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }
        void NavigateTo<T>(object parameter = null) where T : ViewModelBase;

        void GoBack();
        void GoBackWhile();
        //void OpenWindow<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
    }
}
