﻿using System;
using System.Data.SqlClient;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace SyncTest
{
    class DbSync
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
            SqlConnection serverConn = (SqlConnection) _serverConn.Create();
            DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription(_sScope);
            
            DbSyncTableDescription listDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("List", serverConn);
            scopeDesc.Tables.Add(listDesc);
            DbSyncTableDescription itemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("Item", serverConn);
            scopeDesc.Tables.Add(itemDesc);
            DbSyncTableDescription listItemDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("ListItem", serverConn);
            scopeDesc.Tables.Add(listItemDesc);

            SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(serverConn,scopeDesc);
            serverProvision.SetCreateTableDefault(DbSyncCreationOption.CreateOrUseExisting);

            serverProvision.Apply();
        }

        public void ProvisionClient()
        {
            SqlConnection clientConn = (SqlConnection)_clientConn.Create();
            SqlConnection serverConn = (SqlConnection) _serverConn.Create();

            DbSyncScopeDescription scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(_sScope, serverConn);
            SqlSyncScopeProvisioning clientProvision = new SqlSyncScopeProvisioning(clientConn,scopeDesc);

            clientProvision.Apply();
        }

        public void Sync()
        {
            SqlConnection clientConn = (SqlConnection)_clientConn.Create();
            SqlConnection serverConn = (SqlConnection)_serverConn.Create();

            SyncOrchestrator syncOrchestrator = new SyncOrchestrator
            {
                LocalProvider = new SqlSyncProvider(_sScope, clientConn),
                RemoteProvider = new SqlSyncProvider(_sScope, serverConn),
                Direction = SyncDirectionOrder.DownloadAndUpload
            };

            ((SqlSyncProvider)syncOrchestrator.LocalProvider).ApplyChangeFailed += Program_ApplyChangeFailed;
           
            SyncOperationStatistics syncStats = syncOrchestrator.Synchronize();
           
            Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
            Console.WriteLine("Total Changes Uploaded: " + syncStats.UploadChangesTotal);
            Console.WriteLine("Total Changes Downloaded: " + syncStats.DownloadChangesTotal);
            Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);
            Console.WriteLine(String.Empty);
        }
 
        public void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {           
            Console.WriteLine(e.Conflict.Type);
            Console.WriteLine(e.Error);
        }
      


    }

}