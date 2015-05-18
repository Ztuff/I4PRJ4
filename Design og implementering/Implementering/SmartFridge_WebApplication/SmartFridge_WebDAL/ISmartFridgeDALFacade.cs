﻿using SmartFridge_WebDAL.UnitOfWork;

namespace SmartFridge_WebDAL
{
    //Facade for the DAL.
    public interface ISmartFridgeDALFacade
    {
        //Creates a UoW. Only allows one instance, so if UoW is in use, throws exception.
        IUnitOfWork GetUnitOfWork();
        //Disposes the UnitOfWork. If no UoW was created, throw exception.
        void DisposeUnitOfWork();
        //Name of Database for the app.
        string DatabaseName { get; set; }
    }
}
