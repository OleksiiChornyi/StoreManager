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
    public class ViewsDBViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly ViewsDBView viewsDBView;
        public ViewsDBViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            viewsDBView = new ViewsDBView()
            {
                DataContext = this
            };
            viewsDBView.Show();
        }
        private void LoadData()
        {
            DataTable triggers = _admin.GetDatabaseObjects("ALL_OBJECTS", "OBJECT_NAME", "VIEW");
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
