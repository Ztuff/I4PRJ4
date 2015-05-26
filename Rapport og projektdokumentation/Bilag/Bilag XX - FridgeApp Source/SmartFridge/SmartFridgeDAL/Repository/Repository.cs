using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    /// <summary>
    /// A repository pattern base class, that contains ToList for derived classes.
    /// </summary>
    /// <typeparam name="T">Entity class.</typeparam>
    public abstract class Repository<T> where T : new() 
    {
        protected IContext Context { get; private set; }

        /// <summary>
        /// Injects the IContext Interface.
        /// </summary>
        /// <param name="context"></param>
        protected Repository(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Executes command and returns all selected in command.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <returns>The selected items.</returns>
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

        /// <summary>
        /// Maps the attributes of the entity from the data table, to be inserted in item.
        /// </summary>
        /// <param name="record">The record, for an example command.ExecuteReader().</param>
        /// <param name="entity"></param>
        protected abstract void Map(IDataRecord record, T entity);
        /// <summary>
        /// Inserts the entity into the transaction.
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Insert(T entity);
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Update(T entity);
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Delete(T entity);
        /// <summary>
        /// Gets all entities in the data table.
        /// </summary>
        /// <returns>List of entities.</returns>
        public abstract IEnumerable<T> GetAll();
    }
}
