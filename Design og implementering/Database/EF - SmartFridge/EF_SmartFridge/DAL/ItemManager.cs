using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EF_SmartFridge.Entities;

namespace EF_SmartFridge.DAL
{
    public class ItemManager
    {
        public async Task<Item> Create(Item item)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Items.Add(item);
                await db.SaveChangesAsync();
                return item;
            }
        }

        public async Task<Item> Update(Item item)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Items.Attach(item);

                var updatedItem = db.Entry(item);
                updatedItem.State = EntityState.Modified;

                await db.SaveChangesAsync();
                return updatedItem.Entity;
            }
        }

        public async Task<IEnumerable<Item>> Read()
        {
            using (var db = new SmartFridgeContext())
            {
                var items = await db.Items.ToListAsync();
                return items;
            }
        }

        public async Task<Item> Read(int id)
        {
            using (var db = new SmartFridgeContext())
            {
                var item = await db.Items.FindAsync(id);
                return item;
            }
        }

        public async Task Delete(Item item)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Items.Attach(item);
                db.Items.Remove(item);
                await db.SaveChangesAsync();
            }

        }


    }
}
