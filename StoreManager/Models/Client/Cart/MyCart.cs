using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Client.Cart
{
    internal class MyCart : StoreCartInteraction
    {
        public MyCart(AllUsersInteractions client) : base(client) { }
    }
}
