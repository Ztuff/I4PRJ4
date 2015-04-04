using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF_SmartFridge.Entities;

namespace EF_SmartFridge
{
    public class SmartFridgeContext : DbContext
    {
        public SmartFridgeContext() : base("EFSmartFridgeConn")
        {
            
        }

        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<Item> Items { get; set; }

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
