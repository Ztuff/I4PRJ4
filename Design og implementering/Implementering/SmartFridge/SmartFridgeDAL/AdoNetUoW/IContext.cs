using System.Data;

namespace DataAccessLayer.AdoNetUoW
{
    public interface IContext
    {
        IUnitOfWork CreateUnitOfWork();
        IDbCommand CreateCommand();
        void Dispose();
    }
}
