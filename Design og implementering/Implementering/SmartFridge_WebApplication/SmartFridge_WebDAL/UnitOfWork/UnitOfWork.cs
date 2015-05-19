using System;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebDAL.Repository;
using SmartFridge_WebModels;

namespace SmartFridge_WebDAL.UnitOfWork
{
    /// <summary>
    /// Implements the IUnitOfWork interface and IDisposable.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SFContext _dbContext;
        private bool _disposed = false;

        private IRepository<List> _listRepo;
        private IRepository<ListItem> _listItemRepo;
        private IRepository<Item> _itemRepo;

        #region Properties

        /// <summary>
        /// Repository for List. Returns the same List Repository, or creates a new one, if it's null.
        /// </summary>
        public IRepository<List> ListRepo
        {
            get { return _listRepo ?? (_listRepo = new Repository<List>(_dbContext)); }
        }

        /// <summary>
        /// Repository for ListItem. Returns the same ListItem Repository, or creates a new one, if it's null.
        /// </summary>
        public IRepository<ListItem> ListItemRepo
        {
            get { return _listItemRepo ?? (_listItemRepo = new Repository<ListItem>(_dbContext)); }
        }

        /// <summary>
        /// Repository for Item. Returns the same Item Repository, or creates a new one, if it's null.
        /// </summary>
        public IRepository<Item> ItemRepo
        {
            get { return _itemRepo ?? (_itemRepo = new Repository<Item>(_dbContext)); }
        }

        #endregion

        /// <summary>
        /// Injects the SFContext.
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(SFContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Saves the changes made in the Unit of Work.
        /// </summary>
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Virtual function for Dispose. Disposes the Unit of Work, unless it's already disposed.
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _dbContext.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// Implementation of IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
