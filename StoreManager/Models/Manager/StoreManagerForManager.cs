using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Manager
{
    public class StoreManagerForManager : ManagerStoreInteraction
    {
        public StoreManagerForManager(User user) : base(user, true) { }
    }
}
