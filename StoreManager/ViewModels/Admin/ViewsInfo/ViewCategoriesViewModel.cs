using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Data;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Creating;
using StoreManager.Views.Admin.ViewsInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StoreManager.ViewModels.Admin.ViewsInfo
{
    public class ViewCategoriesViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly CategoriesView categoriesView;
        public ViewCategoriesViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            categoriesView = new CategoriesView()
            {
                DataContext = this
            };
            categoriesView.Show();
        }
        private void LoadData()
        {
            var dataTable = _admin.GetCategoryHierarchy();
            DataGridItemsSource = dataTable;
        }
        #region Properties
        private DataTable _dataGridItemsSource;
        public DataTable DataGridItemsSource
        {
            get => _dataGridItemsSource;
            set
            {
                if (_dataGridItemsSource != value)
                {
                    _dataGridItemsSource = value;
                    OnPropertyChanged(nameof(DataGridItemsSource));
                }
            }
        }
        #endregion
    }
}
