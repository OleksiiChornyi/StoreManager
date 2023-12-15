using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Product
    {
        public Product()
        { }
        public Product(int productID, string SKU, string productName, BinaryContent binaryContent, Category category, int price, int salesCount, Description description)
        {
            ProductID = productID;
            this.SKU = SKU;
            ProductName = productName;
            BinaryContent = binaryContent;
            Category = category;
            Price = price;
            SalesCount = salesCount;
            Description = description;
        }
        public int ProductID { get; set; }
        public string SKU {  get; set; }
        public string ProductName { get; set; }
        public BinaryContent BinaryContent{ get; set; }
        public Category Category { get; set; }
        public int Price { get; set; }
        public int SalesCount { get; set; }
        public Description Description { get; set; }
    }
}
