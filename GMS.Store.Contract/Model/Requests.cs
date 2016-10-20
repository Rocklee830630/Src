using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Framework.Contract;

namespace GMS.Store.Contract
{
    public class StoreDpRequests:Request
    {
        public int dpid { get; set; }
        public string clmc { get; set; }
    }

    public class StoreTreeNameRequests:Request
    {
        public Guid treeID { get; set; }
    }

    public class StoreTableRequest:Request
    {
        public Guid Gid { get; set; }
        public string Name { get; set; }
        public string orderType { get; set; }
        public string clmc { get; set; }
    }

    public class OrderRequest : Request
    {
        public string Name { get; set; } 
    }

    public class OutBoundRequest:Request
    {
        public string khmc { get; set; }
        public string ddmc { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }
    public class InBoundRequest:Request
    {
        public string khmc { get; set; }
        public string ddmc { get; set; }
        public string clmc { get; set; } 
        public string czlx { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }
}
