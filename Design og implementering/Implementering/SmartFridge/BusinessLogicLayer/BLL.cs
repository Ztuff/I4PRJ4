﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.AdoNetUoW;
using DataAccessLayer.Connection;
using DataAccessLayer.Repository;
using InterfacesAndDTO;

namespace BusinessLogicLayer
{
    public class BLL
    {
        private IConnectionFactory _connectionFactory = new AppConnectionFactory("SmartFridgeConn");
        public readonly IContext Context;
        private ItemRepository _itemRepository;
        private ListRepository _listRepository;
        private ListItemRepository _listItemRepository;
        public ObservableCollection<GUIItemList> Lists { get; private set; }
        private List<Item> _dbItems;
        private List<ListItem> _dblistItems;

        public BLL()
        {
            Context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(Context);
            _listRepository = new ListRepository(Context);
            _listItemRepository = new ListItemRepository(Context);

            LoadFromDB();
        }

        public readonly ObservableCollection<string> UnitNames = new ObservableCollection<string>()
        {
            "l",
            "dl",
            "cl",
            "ml",
            "kg",
            "g"
        };

        public ObservableCollection<GUIItem> Types
        {
            get
            {
                ObservableCollection<GUIItem> guiItems = new ObservableCollection<GUIItem>();

                foreach (var dbItem in _dbItems)
                {
                    GUIItem guiItem = new GUIItem();

                    guiItem.Type = dbItem.ItemName;
                    guiItem.Unit = dbItem.StdUnit;
                    guiItem.Size = (uint)dbItem.StdVolume;

                    guiItems.Add(guiItem);
                }

                return guiItems;
            }
        }

        private void LoadFromDB()
        {
            List<List> lists = _listRepository.GetAll().ToList();
            _dbItems = _itemRepository.GetAll().ToList();
            _dblistItems = _listItemRepository.GetAll().ToList();

            _listItemRepository.Mapper(_dbItems, lists, _dblistItems);

            // Oprettes som ObservableCollection, da den skal bruges direkte af GUI
            Lists = new ObservableCollection<GUIItemList>();
            foreach (var list in lists)
            {
                Lists.Add(new GUIItemList(list.ListId, list.ListName));
            }

            foreach (var list in Lists)
            {
                foreach (var listItem in _dblistItems)
                {
                    if( listItem.List.ListId == list.ID)
                        foreach (var dbItem in _dbItems)
                        {
                            if (listItem.Item.ItemId == dbItem.ItemId)
                            {
                                GUIItem guiItem = new GUIItem()
                                {
                                    Amount = (uint) listItem.Amount,
                                    Type = dbItem.ItemName,
                                    Size = (uint) listItem.Volume,
                                    Unit = listItem.Unit
                                };
                                list.ItemList.Add(guiItem);
                                break;
                            }
                        }
                }                
            }

        }

        public GUIItem CreateNewItem(string type, uint amount, uint size, string unit)
        {
            GUIItem item = new GUIItem();
            item.Type = type;
            item.Amount = amount;
            item.Size = size;
            item.Unit = unit;
            return item;
        }

        public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> newItems)
        {

            GUIItemList currentList = null;

            foreach (var list in Lists)
            {
                if (list.Name == currentListName)
                {
                    currentList = list;
                }
            }

            if (currentList == null)
                throw new Exception();

            bool newItemAdded = false;

            foreach (var newItem in newItems)
            {
                if (NewItem(newItem))
                {
                    Item dbItem = new Item()
                    {
                        ItemName = newItem.Type,
                        StdUnit = newItem.Unit,
                        StdVolume = (int)newItem.Size
                    };
                    _itemRepository.Insert(dbItem);
                    newItemAdded = true;
                }
            }

            if (newItemAdded)
                LoadFromDB();

            foreach (var newItem in newItems)
            {
                bool itemDoesNotExist = true;
                foreach (var item in currentList.ItemList)
                {
                    if (item.Type == newItem.Type)
                    {
                        itemDoesNotExist = false;
                        break;
                    }
                }
                if (itemDoesNotExist)
                {
                    foreach (var dbItem in _dbItems)
                    {
                        if (dbItem.ItemName == newItem.Type)
                        {
                            var listKey = new List()
                            {
                                ListId = currentList.ID,
                                ListName = currentList.Name
                            };

                            ListItem listItem = new ListItem()
                            {
                                Item = dbItem,
                                List = listKey,
                                Amount = (int) newItem.Amount,
                                Unit = newItem.Unit,
                                Volume = (int) newItem.Size
                            };
/* Før mapper:*/
                            _listItemRepository.Insert(listItem);
                            _dblistItems.Add(listItem);
                        }
                    }
                }
                else
                {
                    foreach (var dbItem in _dbItems)
                    {
                        if (dbItem.ItemName == newItem.Type)
                        {
                            foreach (var listItem in _dblistItems)
                            {
                                if (dbItem.ItemId == listItem.Item.ItemId &&
                                    listItem.List.ListId == currentList.ID)
                                {
                                    listItem.Amount += (int)newItem.Amount;
// Uncomment hvis update ikke køres automatisk af mapperen
//                                  _listItemRepository.Update(listItem);
                                }
                            }
                        }
                    }
                }
            }
            List<List> dbLists = new List<List>();
            foreach (var guiItemList in Lists)
            {
                dbLists.Add(new List()
                {
                    ListId = guiItemList.ID,
                    ListName = guiItemList.Name
                });
            }
            _listItemRepository.Mapper(_dbItems, dbLists, _dblistItems);
        }

        private bool NewItem(GUIItem item)
        {
            foreach (var dbItem in _dbItems)
            {
                if (item.Type == dbItem.ItemName)
                    return false;
            }
            return true;
        }

        public void DeleteItem(GUIItem GUIitemToDelete)
        {
            /*Henter alle items fra databasen, da der ikke er nogen direkte måde at connecte
                et GUIitem med et dbItem, da GUIitem ikke har noget ID*/
            Item dbItemToDelete = new Item();
            /*Finder det dbItem der svarer til det GUIitem der skal fjernes*/
            foreach (var VARIABLE in _dbItems)
            {
                if (VARIABLE.ItemName == GUIitemToDelete.Type
                    && VARIABLE.StdUnit == GUIitemToDelete.Unit
                    && (uint)VARIABLE.StdVolume == GUIitemToDelete.Size)
                {
                    dbItemToDelete = VARIABLE;
                    break;
                }
            }
            /*Fjerner det ønskede item fra databasen*/
            _itemRepository.Delete(dbItemToDelete);
        }

        public void ChangeItem(GUIItem oldItem, GUIItem newItem)
        {
            foreach (var item in _dbItems)
            {
                if (item.ItemName == oldItem.Type && item.StdUnit == oldItem.Unit && (uint)item.StdVolume == oldItem.Size)
                {
                    item.ItemName = newItem.Type;
                    item.StdUnit = newItem.Unit;
                    item.StdVolume = (int)newItem.Size;
                    return;
                }
            }
            //evt throw exception her - eller lav returtype om til bool og returnér false hvis det gik dårligt...eller noget i den dur
        }
    }
}
