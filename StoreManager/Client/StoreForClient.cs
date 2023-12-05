using StoreManager.Abstract.Classes;
using StoreManager.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Client
{
    internal class StoreForClient : ClientStoreInteraction, IStoreForClient
    {

        public StoreForClient(string UserName, string password, string contactInfo, Role userRole) : base(UserName, password, contactInfo, userRole) { }

        void IStoreForClient.CreateOrder(int orderNumber, int UserId)
        {
            throw new NotImplementedException();
        }

        void IStoreForClient.CreateOrderItem(int orderNumber, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        int IStoreForClient.CreateOrderNumber()
        {
            throw new NotImplementedException();
        }
    }
}
