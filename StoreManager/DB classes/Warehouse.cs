using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Warehouse
    {
        public Warehouse()
        { }
        public int WarehoseID { get; set; }
        public string WarehoseName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int Availability { get; set; }
    }
}
