using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Shipment
    {
        public Shipment()
        { }
        public int SipmentID { get; set; }
        public string SipmentNumber { get; set; }
        public DateTime SipmentDate { get; set; }
        public string SipmentStatus { get; set; }
        public Order Order {  get; set; }
    }
}
