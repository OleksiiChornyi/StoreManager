using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.Core
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected void SetProperty<T>(ref T backingField, T value, string propertyName) where T : class
        {
            string trimmedValue = value?.ToString()?.TrimStart();

            if (!EqualityComparer<T>.Default.Equals(backingField, value) && trimmedValue != null) //!string.IsNullOrEmpty(trimmedValue)
            {
                backingField = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
