using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.ViewsInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StoreManager.ViewModels.Admin.ViewsInfo
{
    public class ViewDescriptionViewModel : ViewModelBase
    {
        public ManagerStoreInteraction _admin;
        private readonly DescriptionView descriptionView;
        public ViewDescriptionViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            LoadData();
            descriptionView = new DescriptionView()
            {
                DataContext = this
            };
            descriptionView.Show();
        }
        private void LoadData()
        {
            var dataTable = _admin.GetDataFromView("DescriptionView");
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
