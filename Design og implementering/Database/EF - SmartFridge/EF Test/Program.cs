using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EF_SmartFridge;
using EF_SmartFridge.Entities;
using EF_SmartFridge.DAL;

namespace EF_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var item = new Item("Test");
            var list = new List("Fridge");
            var listitem = new ListItem(3, 10, "g", list, item);
            using (var db = new SmartFridgeContext())
            {
                db.Items.Add(item);
                db.Lists.Add(list);
                db.ListItems.Add(listitem);
                db.SaveChanges();
            }

            var manage = new ListManager();
            IEnumerable<List> a = manage.Read().Result;
            a.ToList();
            List b = manage.Read(list.ListId).Result;

        }
    }
}
