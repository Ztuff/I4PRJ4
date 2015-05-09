using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    public class ItemRepository : Repository<Item>
    {
        public ItemRepository(IContext context) : base(context)
        {
        }

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

        public override IEnumerable<Item> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From Item";
                return ToList(command);
            }
        }

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

        protected override void Map(IDataRecord record, Item item)
        {
            item.ItemId = (int)record["ItemId"];
            item.ItemName = (string)record["ItemName"];
            item.StdVolume = (int)record["StandardVolume"];
            item.StdUnit = (string)record["StandardUnit"];
        }
    }
}
