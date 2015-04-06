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
        private IContext _context;
        private ItemRepository _itemRepository;
        private ListRepository _listRepository;
        private ListItemRepository _listItemRepository;
        public ObservableCollection<GUIItemList> Lists { get; private set; }
        private List<Item> DBItems;
       private List<ListItem> ListItems;

        public BLL()
        {
            _context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(_context);
            _listRepository = new ListRepository(_context);
            _listItemRepository = new ListItemRepository(_context);

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

                foreach (var dbItem in DBItems)
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
            // Oprettes som ObservableCollection, da den skal bruges direkte af GUI
            Lists = new ObservableCollection<GUIItemList>();
            foreach (var list in _listRepository.GetAll())
            {
                Lists.Add(new GUIItemList(list.ListId, list.ListName));
            }

            DBItems = _itemRepository.GetAll().ToList();
            ListItems = _listItemRepository.GetAll().ToList();
            foreach (var list in Lists)
            {
                foreach (var listItem in ListItems)
                {
                    if( listItem.List.ListId == list.ID)
                        foreach (var dbItem in DBItems)
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

        public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> items)
        {
            GUIItemList currentList = null;

            foreach (var list in Lists)
            {
                if (list.Name == currentListName)
                {
                    currentList = list;
                    foreach (var item in items)
                    {
                        currentList.ItemList.Add(item);
                    }
                    break;
                }
            }

            if(currentList == null)
                throw new Exception();
        }

        public void DeleteItem(GUIItem GUIitemToDelete)
        {
            /*Henter alle items fra databasen, da der ikke er nogen direkte måde at connecte
                et GUIitem med et dbItem, da GUIitem ikke har noget ID*/
            Item dbItemToDelete = new Item();
            /*Finder det dbItem der svarer til det GUIitem der skal fjernes*/
            foreach (var VARIABLE in DBItems)
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
            foreach (var item in DBItems)
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
