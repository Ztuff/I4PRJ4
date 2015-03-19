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
using SmartFridgeDAL.AdoNetUoW;
using SmartFridgeDAL.Repository;

namespace SmartFridgeDALApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List list = new List {ListName = "John"};
            Item item = new Item {ItemName = "JohnItem", StdUnit = "g", StdVolume = 500};
            var factory = new AppConnectionFactory("SmartFridgeConn");
            List<List> lists;

            var context = new AdoNetContext(factory);

            using (var uow = context.CreateUnitOfWork())
            {
                var listRepos = new ListRepository(context);

                lists = listRepos.GetByName("Carsten").ToList();
            }
            foreach (var i in lists)
            {
                Console.WriteLine(i.ListName);
            }
            list.ListName = "UpdateJohn";
            using (var uow = context.CreateUnitOfWork())
            {
                var listRepos = new ItemRepository(context);
                listRepos.Insert(item);
                uow.SaveChanges();
            }
            context.Dispose();

        }
    }
}
