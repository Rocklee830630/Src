using System;
using GMS.Framework.Contract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GMS.Store.Contract
{
    [Table("DictionaryProperty")]
    public partial class DictionaryProperty : ModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DictionaryProperty()
        {
            InBoundRecord = new HashSet<InBoundRecord>();
            StoreTable = new HashSet<StoreTable>();
        }
 
        [Key]
        public Guid dpid { get; set; }

        public Guid? leaf_id { get; set; }

        [StringLength(50,ErrorMessage ="材料名称不能为空")]
        public string clmc { get; set; } 

        [StringLength(50)]
        public string pm { get; set; }

        [StringLength(50)]
        public string mf { get; set; }
         
        public decimal? js { get; set; }
          
        public virtual DictionaryTree DictionaryTree { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InBoundRecord> InBoundRecord { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTable> StoreTable { get; set; }
    }
}
