using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    public abstract class Repository<T> where T : new() 
    {
        protected IContext Context { get; private set; }

        protected Repository(IContext context)
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
        public abstract void Insert(T entity);
        public abstract void Update(T entity);
        public abstract void Delete(T entity);
        public abstract IEnumerable<T> GetAll();

    }
}
