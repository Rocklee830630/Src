using GMS.Framework.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace GMS.Store.Contract
{
    public partial class StoreWindowModel : ModelBase
    {
        public string khmc { get; set; }
        public string ddlx { get; set; }
        public string clmc { get; set; }
        public string pm { get; set; }
        public string mf { get; set; }
        public string js { get; set; }

        [RegularExpression(@"^[0-9]+(.[0-9]{2})?$", ErrorMessage = "请输入正确的库存量！")]
        public string number { get; set; }
    }
}
