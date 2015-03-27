using System.Data;

namespace DataAccessLayer.Connection
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
