using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EF_SmartFridge.Entities;

namespace EF_SmartFridge.DAL
{
    public class ListManager
    {
        public async Task<List> Create(List list)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Lists.Add(list);
                await db.SaveChangesAsync();
                return list;
            }
        }

        public async Task<List> Update(List list)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Lists.Attach(list);

                var updatedList = db.Entry(list);
                updatedList.State = EntityState.Modified;

                await db.SaveChangesAsync();
                return updatedList.Entity;
            }
        }

        //Reads only lists
        public async Task<IEnumerable<List>> Read()
        {
            using (var db = new SmartFridgeContext())
            {
                var lists = await db.Lists.ToListAsync();
                return lists;
            }
        }

        //Reads list and content from id
        public async Task<List> Read(int id)
        {
            using (var db = new SmartFridgeContext())
            {
                var list = await db.Lists.Include(l => l.ListItems.Select(li => li.Item)).SingleOrDefaultAsync(l => l.ListId == id);
                return list;
            }
        }

        public async Task Delete(List list)
        {
            using (var db = new SmartFridgeContext())
            {
                db.Lists.Attach(list);
                db.Lists.Remove(list);
                await db.SaveChangesAsync();
            }
        }

    }
}
