using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Category
    {
        public Category()
        { }

        public Category(string categoryNameText, string categoryDescriptionText, int? parentCategoryId)
        {
            this.CategoryName = categoryNameText;
            this.CategoryDescription = categoryDescriptionText;
            this.ParentCategoryID = parentCategoryId;
        }

        public Category(int categoryID, string categoryName, string categoryDescription, int? parentCategoryID)
        {
            CategoryID = categoryID;
            CategoryName = categoryName;
            CategoryDescription = categoryDescription;
            ParentCategoryID = parentCategoryID;
        }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int? ParentCategoryID { get; set; }
        public bool IsCategoryInHierarchy(int categoryId, List<Category> categoryItems)
        {
            var category = categoryItems.FirstOrDefault(c => c.CategoryID == this.CategoryID);

            if (category == null)
            {
                return false;
            }

            while (category != null)
            {
                if (category.CategoryID == categoryId)
                {
                    return true;
                }

                category = categoryItems.FirstOrDefault(c => c.CategoryID == category.ParentCategoryID);
            }

            return false;
        }
    }
}
