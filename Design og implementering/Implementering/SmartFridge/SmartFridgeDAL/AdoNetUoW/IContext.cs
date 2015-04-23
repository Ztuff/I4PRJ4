using System;
using System.Data;

namespace DataAccessLayer.AdoNetUoW
{
    public interface IContext : IDisposable
    {
        IUnitOfWork CreateUnitOfWork();
        IDbCommand CreateCommand();
    }
}
