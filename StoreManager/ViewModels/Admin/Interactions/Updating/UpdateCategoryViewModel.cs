using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.Core;
using StoreManager.Views.Admin.Interactions.Creating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using StoreManager.Views.Admin.Interactions.Updating;
using StoreManager.DB_classes;

namespace StoreManager.ViewModels.Admin.Interactions.Updating
{
    public class UpdateCategoryViewModel : ViewModelBase
    {
        private readonly ManagerStoreInteraction _admin;
        private readonly UpdateCategoryView updateCategoryView;
        public UpdateCategoryViewModel(ManagerStoreInteraction admin)
        {
            _admin = admin ?? throw new ArgumentNullException(nameof(admin));

            UpdateCategoryButtonCommand = new RelayCommand(UpdateCategory);
            LoadData();

            updateCategoryView = new UpdateCategoryView()
            {
                DataContext = this
            };
            updateCategoryView.Show();
        }


        private void LoadData()
        {
            DataTable categoriesTable = _admin.GetDataFromView("CategoriesView");
            foreach (DataRow row in categoriesTable.Rows)
            {
                Category category;
                int parentCategoryID = 0;
                if (int.TryParse(row[3].ToString(), out parentCategoryID))
                    category = new Category(int.Parse(row[0].ToString()), row[1].ToString(), row[2].ToString(), parentCategoryID);
                else
                    category = new Category(int.Parse(row[0].ToString()), row[1].ToString(), row[2].ToString(), null);
                ComboBoxCurrentCategoryItemsSource.Add(new ComboBoxItem
                {
                    Tag = category,
                    Content = row[1]
                });
                ComboBoxParentCategoryItemsSource.Add(new ComboBoxItem
                {
                    Tag = category,
                    Content = row[1]
                });
            }
        }

        #region Properties

        private string _categoryNameText;
        public string CategoryNameText
        {
            get => _categoryNameText;
            set => SetProperty(ref _categoryNameText, value, nameof(CategoryNameText));
        }

        private string _categoryDescriptionText;
        public string CategoryDescriptionText
        {
            get => _categoryDescriptionText;
            set => SetProperty(ref _categoryDescriptionText, value, nameof(CategoryDescriptionText));
        }

        private ObservableCollection<ComboBoxItem> _comboBoxParentCategoryItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxParentCategoryItemsSource
        {
            get => _comboBoxParentCategoryItemsSource;
            set
            {
                if (_comboBoxParentCategoryItemsSource != value)
                {
                    _comboBoxParentCategoryItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxParentCategoryItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxParentCategorySelectedItem;
        public ComboBoxItem ComboBoxParentCategorySelectedItem
        {
            get => _comboBoxParentCategorySelectedItem;
            set
            {
                if (_comboBoxParentCategorySelectedItem != value)
                {
                    _comboBoxParentCategorySelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxParentCategorySelectedItem));
                }
            }
        }
        private ObservableCollection<ComboBoxItem> _comboBoxCurrentCategoryItemsSource = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ComboBoxCurrentCategoryItemsSource
        {
            get => _comboBoxCurrentCategoryItemsSource;
            set
            {
                if (_comboBoxCurrentCategoryItemsSource != value)
                {
                    _comboBoxCurrentCategoryItemsSource = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentCategoryItemsSource));
                }
            }
        }

        private ComboBoxItem _comboBoxCurrentCategorySelectedItem;
        public ComboBoxItem ComboBoxCurrentCategorySelectedItem
        {
            get => _comboBoxCurrentCategorySelectedItem;
            set
            {
                if (_comboBoxCurrentCategorySelectedItem != value)
                {
                    Category category = value.Tag as Category;
                    CategoryNameText = category.CategoryName;
                    CategoryDescriptionText = category.CategoryDescription;

                    ComboBoxItem parentCategoryItem = ComboBoxParentCategoryItemsSource
                        .FirstOrDefault(item => (item.Tag as Category)?.CategoryID == category.ParentCategoryID);
                    ComboBoxParentCategorySelectedItem = parentCategoryItem;

                    _comboBoxCurrentCategorySelectedItem = value;
                    OnPropertyChanged(nameof(ComboBoxCurrentCategorySelectedItem));
                }
            }
        }

        #endregion

        #region Commands

        public ICommand UpdateCategoryButtonCommand { get; }

        private void UpdateCategory(object parameter)
        {
            Category CurrentCategory = ComboBoxCurrentCategorySelectedItem.Tag as Category;
            Category parentCategory = ComboBoxParentCategorySelectedItem != null
                ? ComboBoxParentCategorySelectedItem.Tag as Category
                : null;
            string newName = CategoryNameText == null ? CurrentCategory.CategoryName : CategoryNameText;
            string categoryDescriptionText = CategoryDescriptionText == null ? CurrentCategory.CategoryDescription : CategoryDescriptionText;
            var category = new Category(CurrentCategory.CategoryID, newName, categoryDescriptionText, parentCategory?.CategoryID);
            if (CurrentCategory.CategoryID == parentCategory?.CategoryID)
            {
                MessageBox.Show("A category cannot be a parent of itself");
                return;
            }
            if (_admin.UpdateCategory(category))
            {
                MessageBox.Show("Category updated");
                updateCategoryView.Close();
            }
            else
            {
                MessageBox.Show("Can`t update this category");
            }
        }

        #endregion
    }
}
