using GMS.Core.Config;
using GMS.Framework.DAL;
using System.Data.Entity;
using GMS.Store.Contract;
using GMS.Core.Log;

namespace GMS.Store.DAL
{
    public class StoreDbContext : DbContextBase
    {
        public StoreDbContext()
            : base(CachedConfigContext.Current.DaoConfig.Store, new LogDbContext())
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<StoreDbContext>(null);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DictionaryProperty>()
                .HasMany(e => e.InBoundRecord)
                .WithOptional(e => e.DictionaryProperty)
                .HasForeignKey(e => e.inbound_id);

            modelBuilder.Entity<DictionaryProperty>()
                .HasMany(e => e.StoreTable)
                .WithOptional(e => e.DictionaryProperty)
                .HasForeignKey(e => e.store_item_id);

            modelBuilder.Entity<DictionaryTree>()
                .HasMany(e => e.DictionaryProperty)
                .WithOptional(e => e.DictionaryTree)
                .HasForeignKey(e => e.leaf_id);

            modelBuilder.Entity<DictionaryTree>()
                .HasMany(e => e.OutBoundRecord)
                .WithOptional(e => e.DictionaryTree)
                .HasForeignKey(e => e.node_id);

            
        }

        public virtual DbSet<DictionaryProperty> DictionaryProperty { get; set; }
        public virtual DbSet<DictionaryTree> DictionaryTree { get; set; }
        public virtual DbSet<InBoundRecord> InBoundRecord { get; set; }
        public virtual DbSet<OutBoundRecord> OutBoundRecord { get; set; }
        public virtual DbSet<StoreTable> StoreTable { get; set; } 


    }
    
}
