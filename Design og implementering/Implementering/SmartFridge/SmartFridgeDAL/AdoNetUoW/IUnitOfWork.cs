using System;

namespace DataAccessLayer.AdoNetUoW
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
