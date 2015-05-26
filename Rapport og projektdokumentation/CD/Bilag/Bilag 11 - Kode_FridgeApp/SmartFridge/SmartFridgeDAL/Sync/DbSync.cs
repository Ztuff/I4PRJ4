using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Connection;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace DataAccessLayer.Sync
{
    /// <summary>
    /// Class for syncronization of two databases using Sync Framework.
    /// </summary>
    public class DbSync
    {
        private readonly IConnectionFactory _serverConn;
        private readonly IConnectionFactory _clientConn;
        private readonly string _sScope = "SmartFridgeScope";

        /// <summary>
        /// Injects the server and client connections.
        /// </summary>
        /// <param name="serverConn"></param>
        /// <param name="clientConn"></param>
        public DbSync(IConnectionFactory serverConn, IConnectionFactory clientConn)
        {
            _serverConn = serverConn;
            _clientConn = clientConn;
        }

        /// <summary>
        /// Provisions the database acting as server.
        /// </summary>
        public void ProvisionServer()
        {
            var serverConn = (SqlConnection)_serverConn.Create();
            var serverProvision = new SqlSyncScopeProvisioning(serverConn);

            if (serverProvision.ScopeExists(_sScope)) return;

            var scopeDesc = new DbSyncScopeDescription(_sScope);

            var listDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("Lists", serverConn);
            scopeDesc.Tables.Add(listDesc);
            var itemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("Items", serverConn);
            scopeDesc.Tables.Add(itemDesc);
            var listItemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("ListItems",
                serverConn);

            scopeDesc.Tables.Add(listItemDesc);

            serverProvision = new SqlSyncScopeProvisioning(serverConn, scopeDesc);
            serverProvision.SetCreateTableDefault(DbSyncCreationOption.CreateOrUseExisting);
            serverProvision.Apply();
        }

        /// <summary>
        /// Provisions the database acting as client.
        /// </summary>
        public void ProvisionClient()
        {
            var clientConn = (SqlConnection)_clientConn.Create();
            var serverConn = (SqlConnection)_serverConn.Create();

            var clientProvision = new SqlSyncScopeProvisioning(clientConn);
            if (clientProvision.ScopeExists(_sScope)) return;

            var scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(_sScope, serverConn);
            clientProvision = new SqlSyncScopeProvisioning(clientConn, scopeDesc);
            clientProvision.Apply();
        }

        /// <summary>
        /// Syncronizes the databases.
        /// </summary>
        public void Sync()
        {
            var clientConn = (SqlConnection)_clientConn.Create();
            var serverConn = (SqlConnection)_serverConn.Create();

            var syncOrchestrator = new SyncOrchestrator
            {
                LocalProvider = new SqlSyncProvider(_sScope, clientConn),
                RemoteProvider = new SqlSyncProvider(_sScope, serverConn),
                Direction = SyncDirectionOrder.DownloadAndUpload
            };
            
            syncOrchestrator.Synchronize();
        }
    }
}
