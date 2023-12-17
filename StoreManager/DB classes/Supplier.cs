using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Supplier
    {
        public Supplier()
        { }
        public int SupplierID { get; set; }
        public string CompanyName { get; set; }
        public string ContactInfo { get; set; }
        public string SupplierAddress { get; set; }
    }
}
