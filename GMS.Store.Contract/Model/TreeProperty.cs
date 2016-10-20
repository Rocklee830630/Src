using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMS.Store.Contract
{
    public class TreeProperty
    {
        public Guid id { get; set; }
        public Guid pid { get; set; }
        public string name { get; set; }

        public bool type { get; set; }
        public string LinkUrl { get; set; }

    }
    
    public struct IboundProperty
    {
        public string khmc { get; set; }

        public List<string> dpid { get; set; } //材料ID识别相同材料名称用
        public List<string> clmc { get; set; }
        public List<string> pm { get; set; }
        public List<string> mf { get; set; }
        public List<string> js { get; set; } 
        public List<int> numbers { get; set; }

        public int number { get; set; }//允许出库的数量
        public string minclmc { get; set; }//允许出库最少的材料的名称
    }
}
