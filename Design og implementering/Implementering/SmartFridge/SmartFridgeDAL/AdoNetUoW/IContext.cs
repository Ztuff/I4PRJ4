using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFridgeDAL.AdoNetUoW
{
    public interface IContext
    {
        IUnitOfWork CreateUnitOfWork();
        IDbCommand CreateCommand();
        void Dispose();
    }
}
