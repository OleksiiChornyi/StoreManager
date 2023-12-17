using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.ViewsInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.CLient
{
    public class ViewMyOrdersViewModel : ViewModelBase
    {
        public AllUsersInteractions _account;
        private readonly ViewOrdersView viewOrdersView;
        public ViewMyOrdersViewModel(AllUsersInteractions admin)
        {
            _account = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            viewOrdersView = new ViewOrdersView()
            {
                DataContext = this
            };
            viewOrdersView.Show();
        }
        private void LoadData()
        {
            var dataTable = _account.SelectMyUserData();
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
