using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Abstract.Interfaces
{
    internal interface IStoreForClient
    {
        void CreateOrder(int orderNumber, int UserId);
        void CreateOrderItem(int orderNumber, int productId, int quantity);
        int CreateOrderNumber();
    }
}
