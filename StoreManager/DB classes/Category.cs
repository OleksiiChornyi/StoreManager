using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int? ParentCategoryID { get; set; }
        public bool IsCategoryInHierarchy(int categoryId, List<Category> categoryItems)
        {
            var category = categoryItems.FirstOrDefault(c => c.CategoryID == categoryId);

            if (category == null)
            {
                return false;
            }

            while (category != null)
            {
                if (category.CategoryID == this.CategoryID)
                {
                    return true;
                }

                category = categoryItems.FirstOrDefault(c => c.CategoryID == category.ParentCategoryID);
            }

            return false;
        }
    }
}
