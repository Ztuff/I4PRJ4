using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    /// <summary>
    /// A repository pattern derived class for item.
    /// </summary>
    public class ItemRepository : Repository<Item>
    {
        /// <summary>
        /// Injects the IContext interface.
        /// </summary>
        /// <param name="context"></param>
        public ItemRepository(IContext context) : base(context)
        {
        }

        /// <summary>
        /// Inserts an item.
        /// </summary>
        /// <param name="item"></param>
        public override void Insert(Item item)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DECLARE @InsertItem TABLE ([ItemId] INT); INSERT INTO [Item] ([ItemName],[StandardVolume],[StandardUnit]) OUTPUT [inserted].ItemId INTO @InsertItem VALUES(@ItemName,@StandardVolume,@StandardUnit); SELECT * FROM @InsertItem";
                var nameParam = command.CreateParameter();
                nameParam.ParameterName = "@ItemName";
                nameParam.Value = item.ItemName;
                command.Parameters.Add(nameParam);
                var volParam = command.CreateParameter();
                volParam.ParameterName = "@StandardVolume";
                volParam.Value = item.StdVolume;
                command.Parameters.Add(volParam);
                var unitParam = command.CreateParameter();
                unitParam.ParameterName = "@StandardUnit";
                unitParam.Value = item.StdUnit;
                command.Parameters.Add(unitParam);
                item.ItemId = (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Updates an item.
        /// </summary>
        /// <param name="item"></param>
        public override void Update(Item item)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"UPDATE Item SET ItemName = @ListName, StandardVolume = @StandardVolume,
                                        StandardUnit = @StandardUnit WHERE ItemId = @ItemId";
                var nameParam = command.CreateParameter();
                nameParam.ParameterName = "@ItemName";
                nameParam.Value = item.ItemName;
                command.Parameters.Add(nameParam);
                var volParam = command.CreateParameter();
                volParam.ParameterName = "@StandardVolume";
                volParam.Value = item.StdVolume;
                command.Parameters.Add(volParam);
                var unitParam = command.CreateParameter();
                unitParam.ParameterName = "@StandardUnit";
                unitParam.Value = item.StdUnit;
                command.Parameters.Add(unitParam);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <param name="item"></param>
        public override void Delete(Item item)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DELETE FROM Item Where ItemId = @ItemId";
                var param = command.CreateParameter();
                param.ParameterName = "@ItemId";
                param.Value = item.ItemId;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>List of all items.</returns>
        public override IEnumerable<Item> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From Item";
                return ToList(command);
            }
        }

        /// <summary>
        /// Gets an item by name.
        /// </summary>
        /// <param name="name">Name of item.</param>
        /// <returns>Item.</returns>
        public IEnumerable<Item> GetByName(string name)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Item WHERE ItemName = @ItemName";
                var param = command.CreateParameter();
                param.ParameterName = "@ItemName";
                param.Value = name;
                command.Parameters.Add(param);
                return ToList(command);
            }
        }

        /// <summary>
        /// Maps the item attributes.
        /// </summary>
        /// <param name="record">Attributes from data table.</param>
        /// <param name="item">Item to get attributes inserted.</param>
        protected override void Map(IDataRecord record, Item item)
        {
            item.ItemId = (int)record["ItemId"];
            item.ItemName = (string)record["ItemName"];
            item.StdVolume = (int)record["StandardVolume"];
            item.StdUnit = (string)record["StandardUnit"];
        }
    }
}
