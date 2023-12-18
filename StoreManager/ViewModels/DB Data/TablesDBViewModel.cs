using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.ViewsInfo;
using StoreManager.Views.DB_Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.DB_Data
{
    internal class TablesDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly TablesDBView tablesDBView;
        public TablesDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            tablesDBView = new TablesDBView()
            {
                DataContext = this
            };
            tablesDBView.Show();
        }
        private void LoadData()
        {
            var triggers = _admin.GetDatabaseObjects("ALL_TABLES", "TABLE_NAME", "TABLE");
            DataGridItemsSource = triggers;
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
