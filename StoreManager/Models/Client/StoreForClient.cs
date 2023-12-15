using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Client
{
    internal class StoreForClient : UserStoreInteraction, IStoreForClient
    {

        public StoreForClient(User user, bool isSignIn) : base(user, isSignIn) { }

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
