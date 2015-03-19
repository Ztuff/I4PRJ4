using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridgeDAL.AdoNetUoW;

namespace SmartFridgeDAL.Repository
{
    public class ListItemRepository : Repository<ListItem>
    {
        public ListItemRepository(AdoNetContext context) : base(context)
        {
        }

        public void Insert(ListItem listItem)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"INSERT INTO ListItem () OUTPUT INSERTED.ListId VALUES(@ListName)";
                var param = command.CreateParameter();
                param.ParameterName = "@ListName";
                param.Value = listItem.Item.ItemId;
                command.Parameters.Add(param);
                list.ListId = (int)command.ExecuteScalar();
            }
        }

        protected override void Map(IDataRecord record, ListItem list)
        {
            throw new NotImplementedException();
        }
    }
}
