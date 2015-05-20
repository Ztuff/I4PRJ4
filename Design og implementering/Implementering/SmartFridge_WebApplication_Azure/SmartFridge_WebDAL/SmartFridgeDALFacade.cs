using System;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebDAL.UnitOfWork;

namespace SmartFridge_WebDAL
{
    /// <summary>
    /// Implements the ISmartFridgeDALFacade interface. Facade for the DAL.
    /// </summary>
    public class SmartFridgeDALFacade : ISmartFridgeDALFacade
    {
        private SFContext _context;
        private UnitOfWork.UnitOfWork _unitOfWork;

        public string DatabaseName { get; set; }

        #region Constructors


        public SmartFridgeDALFacade()
        {
            
        }

        /// <summary>
        /// Injects a databaseName. Use if you have a connectionstring in web.config/app.config.
        /// </summary>
        /// <param name="databaseName"></param>
        public SmartFridgeDALFacade(string databaseName)
        {
            DatabaseName = databaseName;
        }

        #endregion

        /// <summary>
        /// Creates a Unit of Work. Only allows one instance, so if UoW is in use, throws exception.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork GetUnitOfWork()
        {
            if (_unitOfWork != null)
                throw new InvalidOperationException("A Unit of Work is already in use.");

            _context = DatabaseName == null ? new SFContext() : new SFContext(DatabaseName);
            
            _unitOfWork = new UnitOfWork.UnitOfWork(_context);
            return _unitOfWork;
        }

        /// <summary>
        /// Disposes the UnitOfWork. If no UoW was created, throw exception.
        /// </summary>
        public void DisposeUnitOfWork()
        {
            if (_unitOfWork == null) return;
            _unitOfWork.Dispose();
            _unitOfWork = null;
            _context = null;
        }
    }
}
