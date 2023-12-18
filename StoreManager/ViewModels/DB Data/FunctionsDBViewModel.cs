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
    public class FunctionsDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly FunctionsDBView functionsDBView;
        public FunctionsDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            functionsDBView = new FunctionsDBView()
            {
                DataContext = this
            };
            functionsDBView.Show();
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
