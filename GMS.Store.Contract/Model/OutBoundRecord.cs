using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Store.Contract
{
    [Table("OutBoundRecord")]
    public partial class OutBoundRecord : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ckid { get; set; }

        public Guid? node_id { get; set; }

        public decimal? number { get; set; }

        public DateTime? out_bound_time { get; set; }

        public virtual DictionaryTree DictionaryTree { get; set; }
    }
}
