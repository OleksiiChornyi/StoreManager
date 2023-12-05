using StoreManager.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Admin
{
    internal class StoreManagerForAdmin : AdminStoreInteraction
    {
        public StoreManagerForAdmin(string userName, string password, string contactInfo) : base(userName, password, contactInfo) { }
    }
}
