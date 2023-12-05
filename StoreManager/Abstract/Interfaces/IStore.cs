using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Abstract.Interfaces
{
    public enum Role { client, guest, admin };
    internal interface IStore
    {
        Role myRole { get; set; }
        bool isOk {  get; set; }
        bool isExist {  get; set; }
    }
}
