using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverConn = new AppConnectionFactory("SmartFridgeConn");
            var clientConn = new AppConnectionFactory("SmartFridgeConn2");
            var sync = new DbSync(serverConn,clientConn);
            //sync.ProvisionServer();
            //sync.ProvisionClient();
            sync.Sync();
        }
    }
}
