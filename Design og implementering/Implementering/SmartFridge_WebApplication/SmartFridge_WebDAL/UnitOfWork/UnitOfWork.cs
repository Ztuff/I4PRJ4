using System;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebDAL.Repository;
using SmartFridge_WebModels;

namespace SmartFridge_WebDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SFContext _dbContext;
        private bool _disposed = false;

        private IRepository<List> _listRepo;
        private IRepository<ListItem> _listItemRepo;
        private IRepository<Item> _itemRepo;

        #region Properties

        public IRepository<List> ListRepo
        {
            get { return _listRepo ?? (_listRepo = new Repository<List>(_dbContext)); }
        }

        public IRepository<ListItem> ListItemRepo
        {
            get { return _listItemRepo ?? (_listItemRepo = new Repository<ListItem>(_dbContext)); }
        }

        public IRepository<Item> ItemRepo
        {
            get { return _itemRepo ?? (_itemRepo = new Repository<Item>(_dbContext)); }
        }

        #endregion

        public UnitOfWork(SFContext context)
        {
            _dbContext = context;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _dbContext.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
