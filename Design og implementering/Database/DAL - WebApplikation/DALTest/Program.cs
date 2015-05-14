using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using DAL.UnitOfWork;

namespace DALTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ISmartFridgeDALFacade dal = new SmartFridgeDALFacade("SmartFridgeDb");

            var uow = dal.GetUnitOfWork();
            var list = new List("testing");
            var item = new Item("Meat");
            uow.ListRepo.Add(list);
            var listitem = new ListItem(1, 500, "g", null, list, item);
            uow.ListItemRepo.Add(listitem);
            uow.SaveChanges();

            listitem.Amount = 400;
            uow.ListItemRepo.Update(listitem);
            uow.SaveChanges();

            dal.DisposeUnitOfWork();
        }
    }
}
