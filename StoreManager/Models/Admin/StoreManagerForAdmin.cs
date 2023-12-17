using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.Models.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Admin
{
    public class StoreManagerForAdmin : AdminStoreInteraction
    {
        public StoreManagerForAdmin(User user) : base(user, true) { }
    }
}
