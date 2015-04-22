using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Connection;

namespace DataAccessLayer.Sync
{
    class DbSync
    {
        private IConnectionFactory _serverConn;
        private IConnectionFactory _clientConn;

        public DbSync(IConnectionFactory serverConn, IConnectionFactory clientConn)
        {
            _serverConn = serverConn;
            _clientConn = clientConn;
        }

        public void ProvisionServer()
        {
            _serverConn.Create();
        }

        public void ProvisionClient()
        {
            _clientConn.Create();
        }



    }
}
