using System.Data;

namespace DataAccessLayer.Connection
{
    /// <summary>
    /// Interface for the connectionfactory.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Creates/opens a connection to the defined database. 
        /// </summary>
        /// <returns></returns>
        IDbConnection Create();
    }
}
