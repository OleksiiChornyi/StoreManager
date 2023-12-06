using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.Data
{
    public class CategoryItem
    {
        public int CategoryID { get; set; }
        public int? ParentCategoryID { get; set; }
        public static bool IsCategoryInHierarchy(int categoryId, int selectedCategoryId, List<CategoryItem> categoryItems)
        {
            var category = categoryItems.FirstOrDefault(c => c.CategoryID == categoryId);

            if (category == null)
            {
                return false;
            }

            while (category != null)
            {
                if (category.CategoryID == selectedCategoryId)
                {
                    return true;
                }

                category = categoryItems.FirstOrDefault(c => c.CategoryID == category.ParentCategoryID);
            }

            return false;
        }
    }
}
