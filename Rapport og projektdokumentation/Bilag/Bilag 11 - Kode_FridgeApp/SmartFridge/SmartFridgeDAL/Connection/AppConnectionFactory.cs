using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer.Connection
{
    /// <summary>
    /// Connectionfactory for the databases.
    /// </summary>
    public class AppConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory _provider;
        private readonly string _connectionString;
        private readonly string _name;

        /// <summary>
        /// Finds a connectionstring by connectioName in app.config.
        /// </summary>
        /// <param name="connectionName">Connectionname from app.config.</param>
        public AppConnectionFactory(string connectionName)
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName");

            var connStr = ConfigurationManager.ConnectionStrings[connectionName];
            if (connStr == null)
                throw new ConfigurationErrorsException(string.Format("Failed to find the connection named {0} in App.config",connectionName));

            _name = connStr.ProviderName;
            _provider = DbProviderFactories.GetFactory(connStr.ProviderName);
            _connectionString = connStr.ConnectionString;
        }

        /// <summary>
        /// Creates/opens a connection to the specified database.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Create()
        {
            var connection = _provider.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
    }
}
