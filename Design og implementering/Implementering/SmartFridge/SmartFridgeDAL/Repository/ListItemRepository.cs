using System;
using System.Collections.Generic;
using System.Data;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    public class ListItemRepository : Repository<ListItem>
    {
        public ListItemRepository(IContext context) : base(context)
        {
        }

        public override void Insert(ListItem listItem)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"INSERT INTO ListItem (ListId,ItemId,Amount,Volume,Unit) VALUES(@ListId,@ItemId,@Amount,@Volume,@Unit)";
                var listParam = command.CreateParameter();
                listParam.ParameterName = "@ListId";
                listParam.Value = listItem.List.ListId;
                command.Parameters.Add(listParam);
                var itemParam = command.CreateParameter();
                itemParam.ParameterName = "@ItemId";
                itemParam.Value = listItem.Item.ItemId;
                command.Parameters.Add(itemParam);
                var amountParam = command.CreateParameter();
                amountParam.ParameterName = "@Amount";
                amountParam.Value = listItem.Amount;
                command.Parameters.Add(amountParam);
                var volumeParam = command.CreateParameter();
                volumeParam.ParameterName = "@Volume";
                volumeParam.Value = listItem.Volume;
                command.Parameters.Add(volumeParam);
                var unitParam = command.CreateParameter();
                unitParam.ParameterName = "@Unit";
                unitParam.Value = listItem.Unit;
                command.Parameters.Add(unitParam);
                command.ExecuteNonQuery();
            }
        }

        //No update of ListItem: Delete -> Add if needed.
        public override void Update(ListItem item) 
        {
            throw new NotImplementedException();
        }

        public override void Delete(ListItem listItem)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DELETE FROM Item Where ListId = @ListId AND ItemId = @ItemId AND Amount = @Amount AND Volume = @Volume AND Unit = @Unit";
                var listParam = command.CreateParameter();
                listParam.ParameterName = "@ListId";
                listParam.Value = listItem.List.ListId;
                command.Parameters.Add(listParam);
                var itemParam = command.CreateParameter();
                itemParam.ParameterName = "@ItemId";
                itemParam.Value = listItem.Item.ItemId;
                command.Parameters.Add(itemParam);
                var amountParam = command.CreateParameter();
                amountParam.ParameterName = "@Amount";
                amountParam.Value = listItem.Amount;
                command.Parameters.Add(amountParam);
                var volumeParam = command.CreateParameter();
                volumeParam.ParameterName = "@Volume";
                volumeParam.Value = listItem.Volume;
                command.Parameters.Add(volumeParam);
                var unitParam = command.CreateParameter();
                unitParam.ParameterName = "@Unit";
                unitParam.Value = listItem.Unit;
                command.Parameters.Add(unitParam);
                command.ExecuteNonQuery();
            }
        }

        public override IEnumerable<ListItem> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From ListItem";
                return ToList(command);
            }
        }

        public IEnumerable<ListItem> GetListItemsOnList(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM ListItem WHERE ListId = @ListId";
                var param = command.CreateParameter();
                param.ParameterName = "@ListId";
                param.Value = list.ListId;
                command.Parameters.Add(param);
                return ToList(command);
            }
        }

        protected override void Map(IDataRecord record, ListItem listItem)
        {
            //listItem.List.ListId = (int)record["ListId"];
            //listItem.Item.ItemId = (int)record["ItemId"];
            listItem.Amount = (int)record["Amount"];
            listItem.Volume = (int)record["Volume"];
            listItem.Unit = (string) record["Unit"];
        }
    }
}
