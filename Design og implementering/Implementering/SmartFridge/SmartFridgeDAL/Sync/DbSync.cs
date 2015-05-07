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
    public class DbSync
    {
        private readonly IConnectionFactory _serverConn;
        private readonly IConnectionFactory _clientConn;
        private readonly string _sScope = "SmartFridgeScope";

        public DbSync(IConnectionFactory serverConn, IConnectionFactory clientConn)
        {
            _serverConn = serverConn;
            _clientConn = clientConn;
        }

        public void ProvisionServer()
        {
            var serverConn = (SqlConnection)_serverConn.Create();
            var serverProvision = new SqlSyncScopeProvisioning(serverConn);

            if (serverProvision.ScopeExists(_sScope)) return;

            var scopeDesc = new DbSyncScopeDescription(_sScope);

            var listDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("List", serverConn);
            scopeDesc.Tables.Add(listDesc);
            var itemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("Item", serverConn);
            scopeDesc.Tables.Add(itemDesc);
            var listItemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("ListItem",
                serverConn);

            scopeDesc.Tables.Add(listItemDesc);

            serverProvision = new SqlSyncScopeProvisioning(serverConn, scopeDesc);
            serverProvision.SetCreateTableDefault(DbSyncCreationOption.CreateOrUseExisting);
            serverProvision.Apply();
        }

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
