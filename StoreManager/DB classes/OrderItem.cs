using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class OrderItem
    {
        public OrderItem()
        { }
        public int OrderItemID { get; set; }
        public int OrOrderNumber{ get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

    }
}
