﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        public string CurrentList { private get; set; }
        public List<Notification> Notifications { get; set; }
        private List<Item> _dbItems;
        private List<ListItem> _dblistItems;


        public BLL()
        {
            Context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(Context);
            _listRepository = new ListRepository(Context);
            _listItemRepository = new ListItemRepository(Context);
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

        public ObservableCollection<GUIItem> WatchItems
        {
            get
            {
                ObservableCollection<GUIItem> guiItems = new ObservableCollection<GUIItem>();

                LoadFromDB();
                List<string> temp = new List<string>();
                foreach (var list in Lists)
                {
                    temp.Add(list.Name);
                }

                if (!temp.Contains(CurrentList))
                {
                    CreateList();
                }

                foreach (var dbListItem in _dblistItems)
                {
                    if (dbListItem.List.ListName == CurrentList)
                    {
                        foreach (var dbItem in _dbItems)
                        {
                            if (dbListItem.Item.ItemId == dbItem.ItemId)
                            {
                                GUIItem guiItem = new GUIItem();

                                guiItem.Type = dbItem.ItemName;
                                guiItem.Amount = (uint)dbListItem.Amount;
                                guiItem.Unit = dbListItem.Unit;
                                guiItem.Size = (uint)dbListItem.Volume;

                                guiItems.Add(guiItem);
                            }
                        }
                    }


                }

                return guiItems;
            }
        }

        private void CreateList()
        {
            using (var uow = Context.CreateUnitOfWork())
            {
                List temp = new List(CurrentList);
                _listRepository.Insert(temp);

                uow.SaveChanges();
            }
            LoadFromDB();
        }

        public void LoadFromDB()
        {
            List<List> lists = new List<List>();
            using (var uow = Context.CreateUnitOfWork())
            {
                lists = _listRepository.GetAll().ToList();
                _dbItems = _itemRepository.GetAll().ToList();
                _dblistItems = _listItemRepository.GetAll().ToList();

                _listItemRepository.Mapper(_dbItems, lists, _dblistItems);
            }

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
                    if (listItem.List.ListId == list.ID)
                        foreach (var dbItem in _dbItems)
                        {
                            if (listItem.Item.ItemId == dbItem.ItemId)
                            {
                                GUIItem guiItem = new GUIItem()
                                {
                                    Amount = (uint)listItem.Amount,
                                    Type = dbItem.ItemName,
                                    Size = (uint)listItem.Volume,
                                    Unit = listItem.Unit
                                };
                                list.ItemList.Add(guiItem);
                                break;
                            }
                        }
                }
            }

        }

        public GUIItemList GetList(string ListName)
        {
            foreach (var guiItemList in Lists)
            {
                if (guiItemList.Name.Equals(ListName))
                    return guiItemList;
            }
            return null;
        }

        public void CheckShelfLife()
        {
            var items = GetList("Køkken");
            foreach (var item in items.ItemList)
            {

                if (item.ShelfLife.Date <= DateTime.Now)
                {
                    string message = item.Type + " blev for gammel d. " + DateTime.Now.Date;
                    Notifications.Add(new Notification(message, DateTime.Now));
                }

            }
        }

        public bool Compare(GUIItem item1, GUIItem item2)
        {
            if (item1.Amount == item2.Amount)
                if (item1.Size == item2.Size)
                    if (item1.Type.Equals(item2.Type))
                        if (item1.Unit.Equals(item2.Unit))
                        {
                            return true;
                        }
            return false;
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
            if(Lists == null)
            {
                CurrentList = currentListName;
                CreateList();
            }

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
            using (var uow = Context.CreateUnitOfWork())
            {
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

                uow.SaveChanges();
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
                    using (var uow = Context.CreateUnitOfWork())
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
                                    Amount = (int)newItem.Amount,
                                    Unit = newItem.Unit,
                                    Volume = (int)newItem.Size
                                };

                                _listItemRepository.Insert(listItem);

                                _dblistItems.Add(listItem);
                            }
                        }
                        uow.SaveChanges();
                    }

                }
                else
                {
                    using (var uow = Context.CreateUnitOfWork())
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
                                        Task.Run(() => _listItemRepository.Update(listItem));
                                    }
                                }
                            }
                        }
                        uow.SaveChanges();
                    }

                }

                LoadFromDB();
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
            STDToShopListControl(currentListName);
        }

        /*public GUIItem GetStandardInfo(GUIItem item)
        {
            return null;
        }*/

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
            using (var uow = Context.CreateUnitOfWork())
            {
                /*Finder det dbItem der svarer til det GUIitem der skal fjernes*/
                foreach (var dbListItem in _dblistItems)
                {

                    if (dbListItem.Item.ItemName == GUIitemToDelete.Type
                        && dbListItem.Amount == GUIitemToDelete.Amount
                        && dbListItem.Unit == GUIitemToDelete.Unit
                        && (uint)dbListItem.Volume == GUIitemToDelete.Size)
                    {
                        /*Fjerner det ønskede item fra databasen*/
                        _listItemRepository.Delete(dbListItem);
                        break;
                    }
                }


                uow.SaveChanges();
            }
        }
        public void ChangeItem(GUIItem oldItem, GUIItem newItem)
        {
            using (var uow = Context.CreateUnitOfWork())
            {
                foreach (var dbListItem in _dblistItems)
                {
                    if (dbListItem.Item.ItemName == oldItem.Type && dbListItem.Unit == oldItem.Unit && (uint)dbListItem.Volume == oldItem.Size && dbListItem.Amount == oldItem.Amount)
                    {
                        if (oldItem.Type != newItem.Type) //Hvis der skal ændres i dens Item (og ikke kun i listItem)
                        {
                            foreach (var dbItem in _dbItems)
                            {
                                if (dbItem.ItemName == oldItem.Type)
                                {
                                    dbItem.ItemName = newItem.Type;
                                    _itemRepository.Update(dbItem);
                                }
                            }
                        }
                        ListItem updatedListItem = new ListItem((int)newItem.Amount, 
                                                                (int)newItem.Size, 
                                                                newItem.Unit,dbListItem.List,
                                                                dbListItem.Item);

                        _listItemRepository.Delete(dbListItem);
                        _listItemRepository.Insert(updatedListItem);

                        uow.SaveChanges();
                        break;
                    }
                }

            }
            //evt throw exception her - eller lav returtype om til bool og returnér false hvis det gik dårligt...eller noget i den dur
        } 
      /*  #region ChangeItem - Old
        public void ChangeItem(GUIItem oldItem, GUIItem newItem)
        {
            using (var uow = Context.CreateUnitOfWork())
            {
                foreach (var ListItemToEdit in _dblistItems)
                {
                    if (ListItemToEdit.Item.ItemName == oldItem.Type && ListItemToEdit.Unit == oldItem.Unit && (uint)ListItemToEdit.Volume == oldItem.Size && ListItemToEdit.Amount == oldItem.Amount)
                    {

                        ListItemToEdit.Item.ItemName = newItem.Type;
                        ListItemToEdit.Unit = newItem.Unit;
                        ListItemToEdit.Volume = (int)newItem.Size;
                        ListItemToEdit.Amount = (int)newItem.Amount;
                        _listItemRepository.Update(ListItemToEdit);
                        uow.SaveChanges();
                        break;
                    }
                }

            }
            //evt throw exception her - eller lav returtype om til bool og returnér false hvis det gik dårligt...eller noget i den dur
        } #endregion*/

        public void STDToShopListControl(string ListWithNewItem)
        {
            if (ListWithNewItem == "Køleskab" || ListWithNewItem == "Indkøbsliste") { return; }
            if (ListWithNewItem == "Standard-beholdning")
            {
                List<ListItem> har = new List<ListItem>();
                List<ListItem> skalHave = new List<ListItem>();
                //Kan ikke gå fra list til vores listItem.
                //Bliver nødt til at lede alle listItems igennem og finde dem der tilhører
                //den rigtige liste.

                using (var uow = Context.CreateUnitOfWork())
                {
                    foreach (var dbListItem in _dblistItems) //Laver en liste med hvad vi har, og hvad der skal være
                    {
                        if (dbListItem.List.ListName == "Køleskab")
                        {
                            har.Add(dbListItem);
                        }
                        else if (dbListItem.List.ListName == "Standard-beholdning")
                        {
                            skalHave.Add(dbListItem);
                        }
                    }
                }
                foreach (var STDListItem in skalHave) //Sammenligner de to lister, og sletter det vi har
                {
                    foreach (var ownedListItem in har)
                    {
                        if (ownedListItem.Item.ItemName == STDListItem.Item.ItemName &&
                            ownedListItem.Amount >= STDListItem.Amount &&
                            ownedListItem.Unit == STDListItem.Unit &&
                            ownedListItem.Volume == STDListItem.Volume)
                        {
                            skalHave.Remove(STDListItem);
                        }
                    }
                }
                ObservableCollection<GUIItem> ItemsToAdd = new ObservableCollection<GUIItem>();
                foreach (var STDListItem in skalHave)
                {
                    GUIItem ItemToAdd = new GUIItem(STDListItem.Item.ItemName,
                                                    (uint)STDListItem.Amount,
                                                    (uint) STDListItem.Volume,
                                                    STDListItem.Unit);
                    ItemsToAdd.Add(ItemToAdd);
                }
                    AddItemsToTable("Indkøbsliste", ItemsToAdd);
            }
            else { throw new Exception("List not recognized");}
        }
    }
}
