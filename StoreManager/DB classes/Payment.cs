using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Payment
    {
        public Payment()
        { }
        public int PaymentID { get; set; }
        public int UserID { get; set; }
        public int OrderNumber { get; set; }
        public Transaction Transaction { get; set; }
        public int TotalPrice { get; set; }
        public int CashID { get; set; }
        public int BankCardID { get; set; }
    }
}
