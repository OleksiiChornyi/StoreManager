using StoreManager.Abstract.Classes;
using StoreManager.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Guest
{
    internal class StoreForGuest : ClientStoreInteraction, IStoreForGuest
    {
        public StoreForGuest(string UserName, string password, string contactInfo, Role userRole) : base(UserName, password, contactInfo, userRole)
        { }
    }
}
