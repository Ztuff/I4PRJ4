using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    /// <summary>
    /// A repository pattern dereved class for list.
    /// </summary>
    public class ListRepository : Repository<List>
    {
        /// <summary>
        /// Injects the IContext interface.
        /// </summary>
        /// <param name="context"></param>
        public ListRepository(IContext context) : base(context)
        {
        }

        /// <summary>
        /// Inserts a list.
        /// </summary>
        /// <param name="list"></param>
        public override void Insert(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DECLARE @InsertList TABLE ([ListId] INT); INSERT INTO [Lists] ([ListName]) OUTPUT [inserted].ListId INTO @InsertList VALUES(@ListName); SELECT * FROM @InsertList";
                var param = command.CreateParameter();
                param.ParameterName = "@ListName";
                param.Value = list.ListName;
                command.Parameters.Add(param);
                list.ListId = (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Updates a list.
        /// </summary>
        /// <param name="list"></param>
        public override void Update(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"UPDATE Lists SET ListName = @ListName WHERE ListId = @ListId";
                var nameParam = command.CreateParameter();
                nameParam.ParameterName = "@ListName";
                nameParam.Value = list.ListName;
                command.Parameters.Add(nameParam);
                var idParam = command.CreateParameter();
                idParam.ParameterName = "@ListId";
                idParam.Value = list.ListId;
                command.Parameters.Add(idParam);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a list.
        /// </summary>
        /// <param name="list"></param>
        public override void Delete(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DELETE FROM Lists Where ListId = @ListId";
                var param = command.CreateParameter();
                param.ParameterName = "@ListId";
                param.Value = list.ListId;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets all lists.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<List> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From Lists";
                return ToList(command);
            }
        }

        /// <summary>
        /// Gets a list by name.
        /// </summary>
        /// <param name="name">Name of list.</param>
        /// <returns></returns>
        public IEnumerable<List> GetByName(string name)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Lists WHERE ListName = @ListName";
                var param = command.CreateParameter();
                param.ParameterName = "@ListName";
                param.Value = name;
                command.Parameters.Add(param);
                return ToList(command);
            }
        }

        /// <summary>
        /// Maps the list attributes.
        /// </summary>
        /// <param name="record">Attributes from data table.</param>
        /// <param name="list">Listitem to get attributes inserted.</param>
        protected override void Map(IDataRecord record, List list)
        {
            list.ListId = (int) record["ListId"];
            list.ListName = (string) record["ListName"];
        }
    }
}
