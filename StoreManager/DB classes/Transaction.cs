using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Transaction
    {
        public Transaction()
        { }
        public int TransactionID { get; set; }
        public int UserID { get; set; }
        public int OrderNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
    }
}
