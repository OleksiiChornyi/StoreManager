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
    public class ViewAuditLogssViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly ViewAuditLogssView viewAuditLogssView;
        public ViewAuditLogssViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            viewAuditLogssView = new ViewAuditLogssView()
            {
                DataContext = this
            };
            viewAuditLogssView.Show();
        }
        private void LoadData()
        {
            var dataTable = _admin.GetDataFromView("AuditLogsViewView");
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
