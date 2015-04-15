using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    public class ListItemRepository : Repository<ListItem>
    {
        public ListItemRepository(IContext context)
            : base(context)
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
            listItem.Unit = (string)record["Unit"];
        }

        public void Mapper(List<Item> items, List<List> lists, List<ListItem> listItems)
        {
            /*using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT LI.ListId, LI.ItemId
                                FROM ListItem AS LI
                                    INNER JOIN List AS L
                                        ON L.ListId = LI.ListId
                                    INNER JOIN Item AS I
                                        ON I.ListId = LI.ItemId";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int listId = (int)reader["ListId"];
                        int itemId = (int)reader["ItemId"];

                        foreach (var listItem in listItems)
                        {
                            listItem.List = lists.SingleOrDefault(l => l.ListId = listItem.ListId);
                            listItem.Item = items.SingleOrDefault(i => i.ItemId = listItem.ItemId);
                        }
                    }
                }*/

                using (var command = Context.CreateCommand())
                {
                    command.CommandText = @"SELECT ListItem.ListId, ListItem.ItemId, Item.ItemId AS ItemItemId, List.ListId AS ListListId 
                                                FROM Item INNER JOIN
                                                ListItem ON Item.ItemId = ListItem.ItemId INNER JOIN
                                                List ON ListItem.ListId = List.ListId";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int listid = (int)reader["ListId"];
                            int itemid = (int)reader["ItemId"];
                            int listlistid = (int)reader["ListId"];
                            int itemitemid = (int)reader["ItemItemId"];

                            foreach (var listitem in listItems)
                            {
                                foreach (var item in items.Where(item => item.ItemId == itemitemid && itemitemid == itemid))
                                {
                                    listitem.Item = item;
                                }

                                foreach (var list in lists.Where(list => list.ListId == listlistid))
                                {
                                    listitem.List = list;
                                }
                            }
                        }
                    }
            }
        }
    }
}
