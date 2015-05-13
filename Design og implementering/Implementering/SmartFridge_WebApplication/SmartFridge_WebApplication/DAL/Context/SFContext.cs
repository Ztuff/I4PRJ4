using System.Data.Entity;
using SmartFridge_WebApplication.Models;


namespace DAL.Context
{
    public class SFContext : DbContext
    {

        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<Item> Items { get; set; }

        #region Constructors

        public SFContext()
        {
        }

        public SFContext(string databaseName)
            : base(databaseName)
        {
        }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ListItem>().HasKey(li => new { li.ListId, li.ItemId });

            modelBuilder.Entity<ListItem>()
                .HasRequired(li => li.List)
                .WithMany(l => l.ListItems)
                .HasForeignKey(li => li.ListId);

            modelBuilder.Entity<ListItem>()
                .HasRequired(li => li.Item)
                .WithMany(l => l.ListItems)
                .HasForeignKey(li => li.ItemId);
        }
    }
}
