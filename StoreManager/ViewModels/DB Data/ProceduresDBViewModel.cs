using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.DB_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.DB_Data
{
    public class ProceduresDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly ProceduresDBView proceduresDBView;
        public ProceduresDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            proceduresDBView = new ProceduresDBView()
            {
                DataContext = this
            };
            proceduresDBView.Show();
        }
        private void LoadData()
        {
            var triggers = _admin.GetDatabaseObjects("ALL_PROCEDURES", "OBJECT_NAME", "PROCEDURE");
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
