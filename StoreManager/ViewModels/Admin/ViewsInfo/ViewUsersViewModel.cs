using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.ViewsInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.Admin.ViewsInfo
{
    public class ViewUsersViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly ViewUsersView viewUsersView;
        public ViewUsersViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            viewUsersView = new ViewUsersView()
            {
                DataContext = this
            };
            viewUsersView.Show();
        }
        private void LoadData()
        {
            var dataTable = _admin.GetDataFromView("UsersView");
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
