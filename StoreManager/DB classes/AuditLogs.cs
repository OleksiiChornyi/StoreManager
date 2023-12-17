using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class AuditLogs
    {
        public AuditLogs()
        { }
        public int LogID { get; set; }
        public string ActionType {  get; set; }
        public DateTime ActionDateTime {  get; set; }
        public string ActionDetails { get; set; }
    }
}
