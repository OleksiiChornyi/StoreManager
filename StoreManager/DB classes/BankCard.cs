using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class BankCard
    {
        public BankCard()
        { }
        public int BankCardID { get; set; }
        public string BankNumber { get; set; }
        public int OrderNumber { get; set; }
    }
}
