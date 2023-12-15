using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.SQL_static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Guest
{
    internal class StoreForGuest : UserStoreInteraction, IStoreForGuest
    {
        public StoreForGuest() : base(
            User.CreateGuest(), 
            true)
        { }
    }
}
