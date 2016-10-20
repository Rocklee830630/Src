using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Store.Contract
{
    [Table("DictionaryTree")]
    public partial class DictionaryTree : ModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DictionaryTree()
        {
            DictionaryProperty = new HashSet<DictionaryProperty>();
            OutBoundRecord = new HashSet<OutBoundRecord>();
        }

        [Key]
        public Guid dt_id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string parent_name { get; set; }
        public Guid parent_id { get; set; }

        public bool? is_leaf { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DictionaryProperty> DictionaryProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OutBoundRecord> OutBoundRecord { get; set; }
    }
}
