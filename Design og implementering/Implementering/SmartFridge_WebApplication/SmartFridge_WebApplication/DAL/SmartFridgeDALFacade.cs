using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridge_WebApplication.DAL.Context;
using SmartFridge_WebApplication.DAL.UnitOfWork;

namespace SmartFridge_WebApplication.DAL
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

            _context = new SFContext(DatabaseName);
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
