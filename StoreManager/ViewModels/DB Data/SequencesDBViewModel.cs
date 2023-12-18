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
    public class SequencesDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly SequencesDBView sequencesDBView;
        public SequencesDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            sequencesDBView = new SequencesDBView()
            {
                DataContext = this
            };
            sequencesDBView.Show();
        }
        private void LoadData()
        {
            DataTable triggers = _admin.GetDatabaseObjects("ALL_OBJECTS", "OBJECT_NAME", "SEQUENCE");
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
