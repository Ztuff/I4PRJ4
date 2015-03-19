using System;

namespace SmartFridgeDAL.AdoNetUoW
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
