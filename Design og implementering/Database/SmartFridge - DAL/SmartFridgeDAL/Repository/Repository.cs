using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SmartFridgeDAL.AdoNetUoW;

namespace SmartFridgeDAL.Repository
{
    public abstract class Repository<T> where T : new()
    {
        protected AdoNetContext Context { get; private set; }

        protected Repository(AdoNetContext context)
        {
            Context = context;
        }

        protected IEnumerable<T> ToList(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                var items = new List<T>();
                while (reader.Read())
                {
                    var item = new T();
                    Map(reader, item);
                    items.Add(item);
                }
                return items;
            }
        }

        protected abstract void Map(IDataRecord record, T list);
    }
}
