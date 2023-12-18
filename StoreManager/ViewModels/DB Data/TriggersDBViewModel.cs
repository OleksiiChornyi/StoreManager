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
    public class TriggersDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly TriggerssDBView triggerssDBView;
        public TriggersDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            triggerssDBView = new TriggerssDBView()
            {
                DataContext = this
            };
            triggerssDBView.Show();
        }
        private void LoadData()
        {
            DataTable triggers = _admin.GetDatabaseObjects("ALL_OBJECTS", "OBJECT_NAME", "TRIGGER");
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
