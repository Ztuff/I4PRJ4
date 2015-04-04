using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF_SmartFridge.Entities;

namespace EF_SmartFridge.DAL
{
    public class ListItemManager
    {
        public async Task<ListItem> Create(ListItem listitem)
        {
            using (var db = new SmartFridgeContext())
            {
                db.ListItems.Add(listitem);
                await db.SaveChangesAsync();
                return listitem;
            }
        }

        public async Task<ListItem> Update(ListItem listitem)
        {
            using (var db = new SmartFridgeContext())
            {
                db.ListItems.Attach(listitem);

                var updatedListItem = db.Entry(listitem);
                updatedListItem.State = EntityState.Modified;

                await db.SaveChangesAsync();
                return updatedListItem.Entity;
            }
        }

        public async Task<IEnumerable<ListItem>> Read()
        {
            using (var db = new SmartFridgeContext())
            {
                var listitems = await db.ListItems.Include(li => li.Item).ToListAsync();
                return listitems;
            }
        }

        public async Task Delete(ListItem listitem)
        {
            using (var db = new SmartFridgeContext())
            {
                db.ListItems.Attach(listitem);
                db.ListItems.Remove(listitem);
                await db.SaveChangesAsync();
            }
        }
    }
}
