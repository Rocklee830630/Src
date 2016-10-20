using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Store.Contract
{
    [Table("StoreTable")]
    public partial class StoreTable : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid kcid { get; set; }

        public Guid? store_item_id { get; set; }

        [RegularExpression(@"^[0-9]+(.[0-9]{2})?$", ErrorMessage="请输入正确的库存数量！")]
        public decimal? number { get; set; }

        public virtual DictionaryProperty DictionaryProperty { get; set; }
    }
}
