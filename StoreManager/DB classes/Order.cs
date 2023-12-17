using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Order
    {
        public Order()
        { }
        public int OrderID { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; }
        public Payment Payment { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public int UserID { get; set; }
    }
}
