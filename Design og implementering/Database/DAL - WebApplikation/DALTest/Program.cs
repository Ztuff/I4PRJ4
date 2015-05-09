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

            uow.ListRepo.Add(new List("testing"));

            uow.SaveChanges();
            
            dal.DisposeUnitOfWork();
        }
    }
}
