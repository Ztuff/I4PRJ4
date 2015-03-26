using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using SmartFridgeDAL.Connection;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using SmartFridgeDAL.AdoNetUoW;
using SmartFridgeDAL.Repository;

namespace SmartFridgeDALApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List list = new List {ListName = "Fridge"};
            Item item = new Item {ItemName = "Milk", StdUnit = "L", StdVolume = 1};
            Item item2 = new Item {ItemName = "Meat", StdUnit = "g", StdVolume = 500};
            
            var factory = new AppConnectionFactory("SmartFridgeConn");

            var context = new AdoNetContext(factory);

            var listRepostest = new ListRepository(context);
            List<List> john = listRepostest.GetAll().ToList();

            ListItem listitem = new ListItem { List = list, Item = item, Amount = 2, Unit = "L", Volume = 1 };
            ListItem listitem2 = new ListItem { List = list, Item = item2, Amount = 3, Unit = "g", Volume = 500 };

            using (var uow = context.CreateUnitOfWork())
            {
                var listRepos = new ListRepository(context);
                var itemRepos = new ItemRepository(context);
                var listitemRepos = new ListItemRepository(context);
                listRepos.Insert(list);
                itemRepos.Insert(item);
                itemRepos.Insert(item2);
                listitemRepos.Insert(listitem);
                listitemRepos.Insert(listitem2);
                uow.SaveChanges();
            }

            List<ListItem> test;
            var listitemRepostest = new ListItemRepository(context);
            test = listitemRepostest.GetListItemsOnList(list).ToList();

            foreach (var i in test)
            {
                Console.WriteLine("Amount: " + i.Amount + ". Volume: " + i.Volume + ". Unit: " + i.Unit);
            }

            List<List> listtest = new List<List>();
            listtest.Add(list);

            List<Item> itemtest = new List<Item>();
            itemtest.Add(item);
            itemtest.Add(item);

            listitemRepostest.Mapper(itemtest,listtest,test);

            int dummy;

        }
    }
}
