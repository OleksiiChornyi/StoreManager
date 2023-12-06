using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Guest
{
    internal class StoreForGuest : ClientStoreInteraction, IStoreForGuest
    {
        public StoreForGuest(string UserName, string password, string contactInfo, Role userRole) : base(UserName, password, contactInfo, userRole)
        { }
    }
}
