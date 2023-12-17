using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Inventory
    {
        public Inventory()
        { }
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public int QuantityOnHand { get; set; }
        public int WareHouseID { get; set; }
        public DateTime InventoryDate {  get; set; }
    }
}
