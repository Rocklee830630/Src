using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Store.Contract
{
    [Table("InBoundRecord")]
    public partial class InBoundRecord : ModelBase
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid rkid { get; set; }

        public Guid? inbound_id { get; set; }

        public decimal? number { get; set; }

        public DateTime? inbound_time { get; set; }

        public decimal? before_number { get; set; }

        public string boundtype { get; set; }

        public string khmc { get; set; }

        public virtual DictionaryProperty DictionaryProperty { get; set; }
    }
}
