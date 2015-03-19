using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridgeDAL.AdoNetUoW;

namespace SmartFridgeDAL.Repository
{
    public class ItemRepository : Repository<Item>
    {
        public ItemRepository(AdoNetContext context) : base(context)
        {
        }

        public void Insert(Item item)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Item (ItemName,StandardVolume,StandardUnit) 
                                        OUTPUT INSERTED.ItemId VALUES(@ItemName,@StandardVolume,@StandardUnit)";
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

        public void Update(Item item)
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

        public void Delete(Item item)
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

        public IEnumerable<Item> GetItems()
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
            item.ItemName = (string)record["ListName"];
            item.StdVolume = (int)record["StandardVolume"];
            item.StdUnit = (string)record["StandardUnit"];
        }
    }
}
