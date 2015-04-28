using System.Data;

namespace SyncTest
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
