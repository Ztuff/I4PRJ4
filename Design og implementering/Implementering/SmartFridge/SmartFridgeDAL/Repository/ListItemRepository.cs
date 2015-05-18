using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataAccessLayer.AdoNetUoW;

namespace DataAccessLayer.Repository
{
    /// <summary>
    /// A repository pattern derived class for listitem.
    /// </summary>
    public class ListItemRepository : Repository<ListItem>
    {
        /// <summary>
        /// Injects the IContext interface.
        /// </summary>
        /// <param name="context"></param>
        public ListItemRepository(IContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Inserts a listitem.
        /// </summary>
        /// <param name="listItem"></param>
        public override void Insert(ListItem listItem)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"INSERT INTO ListItems (ListId,ItemId,Amount,Volume,Unit,ShelfLife) VALUES(@ListId,@ItemId,@Amount,@Volume,@Unit,@ShelfLife)";
                
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

                var shelfParam = command.CreateParameter();
                shelfParam.ParameterName = "@ShelfLife";
                shelfParam.Value = listItem.ShelfLife;
                command.Parameters.Add(shelfParam);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Not implemented function. Delete the old listitem and insert the updated one to update a listitem.
        /// </summary>
        /// <param name="item"></param>
        public override void Update(ListItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a listitem.
        /// </summary>
        /// <param name="listItem"></param>
        public override void Delete(ListItem listItem)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DELETE FROM ListItems Where ListId = @ListId AND ItemId = @ItemId AND Amount = @Amount AND Volume = @Volume AND Unit = @Unit";
                
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

        /// <summary>
        /// Gets all listitems.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ListItem> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From ListItems";
                return ToList(command);
            }
        }

        /// <summary>
        /// Gets all listitem from specified list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetListItemsOnList(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM ListItems WHERE ListId = @ListId";
                var param = command.CreateParameter();
                param.ParameterName = "@ListId";
                param.Value = list.ListId;
                command.Parameters.Add(param);
                return ToList(command);
            }
        }

        /// <summary>
        /// Maps the listitem attributes.
        /// </summary>
        /// <param name="record">Attributes from data table.</param>
        /// <param name="listItem">Listitem to get attributes inserted.</param>
        protected override void Map(IDataRecord record, ListItem listItem)
        {
            listItem.ListId = (int)record["ListId"];
            listItem.ItemId = (int)record["ItemId"];
            listItem.Amount = (int)record["Amount"];
            listItem.Volume = (int)record["Volume"];
            listItem.Unit = (string)record["Unit"];
            if(listItem.ShelfLife != null)
                listItem.ShelfLife = (DateTime?)record["ShelfLife"];
        }

        /// <summary>
        /// Maps the references between a list of items, a list of lists and a list of listitems.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="lists"></param>
        /// <param name="listItems"></param>
        public void Mapper(List<Item> items, List<List> lists, List<ListItem> listItems)
        {
            foreach (var listItem in listItems)
            {
                foreach (var item in items.Where(item => item.ItemId == listItem.ItemId))
                {
                    listItem.Item = item;
                }

                foreach (var list in lists.Where(list => list.ListId == listItem.ListId))
                {
                    listItem.List = list;
                }
            }
        }
    }
}
