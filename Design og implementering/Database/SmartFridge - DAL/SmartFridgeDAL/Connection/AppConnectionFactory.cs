﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartFridgeDAL.Connection
{
    public class AppConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory _provider;
        private readonly string _connectionString;
        private readonly string _name;

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

        public IDbConnection Create()
        {
            var connection = _provider.CreateConnection();
            if (connection == null)
                throw new ConfigurationErrorsException(string.Format("Failed to find the connection named {0} in App.config", _name));

            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }

    }
}
