using System;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebDAL.UnitOfWork;

namespace SmartFridge_WebDAL
{
    public class SmartFridgeDALFacade : ISmartFridgeDALFacade
    {
        private SFContext _context;
        private UnitOfWork.UnitOfWork _unitOfWork;

        public string DatabaseName { get; set; }

        #region Constructors

        public SmartFridgeDALFacade()
        {
            
        }

        public SmartFridgeDALFacade(string databaseName)
        {
            DatabaseName = databaseName;
        }

        #endregion

        public IUnitOfWork GetUnitOfWork()
        {
            if (_unitOfWork != null)
                throw new InvalidOperationException("A Unit of Work is already in use.");

            _context = DatabaseName == null ? new SFContext() : new SFContext(DatabaseName);
            
            _unitOfWork = new UnitOfWork.UnitOfWork(_context);
            return _unitOfWork;
        }

        public void DisposeUnitOfWork()
        {
            if (_unitOfWork == null) return;
            _unitOfWork.Dispose();
            _unitOfWork = null;
            _context = null;
        }
    }
}
