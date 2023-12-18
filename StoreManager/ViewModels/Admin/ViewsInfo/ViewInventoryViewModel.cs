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
    //InventoryView
    public class ViewInventoryViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly ViewInventoryView viewInventoryView;
        public ViewInventoryViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            viewInventoryView = new ViewInventoryView()
            {
                DataContext = this
            };
            viewInventoryView.Show();
        }
        private void LoadData()
        {
            var dataTable = _admin.GetDataFromView("InventoryView");
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
