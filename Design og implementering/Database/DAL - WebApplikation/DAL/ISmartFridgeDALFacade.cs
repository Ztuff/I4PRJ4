using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.UnitOfWork;

namespace DAL
{
    //Facade for the DAL.
    interface ISmartFridgeDALFacade
    {
        //Creates a UoW. Only allows one instance, so if UoW is in use, throws exception.
        IUnitOfWork GetUnitOfWork();
        //Disposes the UnitOfWork. If no UoW was created, throw exception.
        void DisposeUnitOfWork();
        //Name of Database for the app.
        string DatabaseName { get; set; }
    }
}
