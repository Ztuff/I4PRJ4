using System;
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
        private List<Item> _dbItems;
        private List<ListItem> _dblistItems;
        public List<Notification> Notifications = new List<Notification>();


        public BLL()
        {
            Context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(Context);
            _listRepository = new ListRepository(Context);
            _listItemRepository = new ListItemRepository(Context);
        }

        public void DeleteItemWithType(string type)
        {
            using (var uow = Context.CreateUnitOfWork())
            {
                foreach (var item in _dbItems)
                {
                    if (item.ItemName.Equals(type))
                    {
                        _itemRepository.Delete(item);
                        break;
                    }
                }
                uow.SaveChanges();
            }
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

        public List<Notification> CheckShelfLife(GUIItemList items)
        {
            var list = new List<Notification>();
            foreach (var item in items.ItemList)
            {

                if (item.ShelfLife.Date <= DateTime.Now)
                { //The item is seen as 'expired' when we're on the same day as its expiration date
                    string message = item.Type + " blev for gammel d. " + DateTime.Now.Date;
                    list.Add(new Notification(message, DateTime.Now, item.ID));
                }

            }
            return list;
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

        public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> newGuiItems)
        {

            GUIItemList currentGuiItemList = null;
            if(Lists == null) //Hvis den liste vi vil tilføje items til ikke findes, opretter vi den OBS: currentList er den lokale, CurrentList er klassens 
            {
                CurrentList = currentListName;
                CreateList();
            }

            foreach (var list in Lists) // Finder den rigtige GuiListe, og sætter vores currentList til den
            {
                if (list.Name == currentListName)
                {
                    currentGuiItemList = list;
                }
            }
            if (currentGuiItemList == null) //Hvis listen ikke er opretet smides en exception
                throw new Exception();
                                                                         
            bool newItemAdded = false;
            using (var uow = Context.CreateUnitOfWork())
            {
                foreach (var newGuiItem in newGuiItems)
                {
                    //IsNewItem ser på om newGuiItem er af en ny type. Altså om det er et helt nyt Item, eller kun et nyt ListItem
                    //Hvis det er en ny type Item bliver dette oprettet i databasen.
                    if (IsNewItem(newGuiItem))
                    {
                        Item dbItem = new Item()
                        {
                            ItemName = newGuiItem.Type,
                            StdUnit = newGuiItem.Unit,
                            StdVolume = (int)newGuiItem.Size
                        };
                        _itemRepository.Insert(dbItem);
                        newItemAdded = true;
                    }
                }

                uow.SaveChanges();
            }


            if (newItemAdded) //Vi Loader data fra databasen, hvis vi har tilføjet det nye item.
                LoadFromDB();

            foreach (var newGuiItem in newGuiItems) //Ser ´på alle elementer i listen af GuiItems der skal tilføjes
            {
                bool itemDoesNotExist = true;
                foreach (var item in currentGuiItemList.ItemList)//Ser på alle GuiItems der allerede er i vores liste
                {
                    if (item.Type == newGuiItem.Type) //Hvis det GUI item vi skal tilføje har samme type, som et af vores eksisterende
                    {                                               //GUI items gør vi intent
                        itemDoesNotExist = false;
                        break;
                    }
                }
                if (itemDoesNotExist) // Hvis det GUI item vi vil tilføje IKKE har samme type som et eksisterende GUI-Item.
                {
                    using (var uow = Context.CreateUnitOfWork())
                    {
                        foreach (var dbItem in _dbItems) //Vi leder nu databasen af items igennem for at finde det Item som 
                        {                                       // har samme navn som typen af vores GUI-Item
                            if (dbItem.ItemName == newGuiItem.Type)
                            {
                                var listKey = new List()
                                {
                                    ListId = currentGuiItemList.ID,
                                    ListName = currentGuiItemList.Name
                                };

                                //Vi har nu fundet det korrekte Item i databasen, og laver nu ListItem'et, og putter i databasen
                                ListItem listItem = new ListItem()
                                {
                                    Item = dbItem,
                                    List = listKey,
                                    Amount = (int)newGuiItem.Amount,
                                    Unit = newGuiItem.Unit,
                                    Volume = (int)newGuiItem.Size
                                };

                                _listItemRepository.Insert(listItem);

                                _dblistItems.Add(listItem);
                            }
                        }
                        uow.SaveChanges();
                    }

                }
                else  // Hvis det GUI item vi vil tilføje har samme type som et eksisterende GUI-Item i vores Liste<GUI-Item>.
                {       //så finder vi ud af hvad forksellen er, og tilføjer en listItem udgave mere, eller tilapasser amount
                    using (var uow = Context.CreateUnitOfWork())
                    {
                        foreach (var dbItem in _dbItems)
                        {
                            if (dbItem.ItemName == newGuiItem.Type) //hvis vi finder det Item der har navn tilsvarende typen på Gui-Item'et
                            {
                                foreach (var dbListItem in _dblistItems) //finder alle de listItems af vores Item type.
                                {//Samme Item, forskellig amount (+/-). Vi sletter det gamle item, og tilføjer et med sammenlagt amount
                                    if (dbItem.ItemId == dbListItem.Item.ItemId &&
                                        dbListItem.List.ListId == currentGuiItemList.ID &&
                                        dbListItem.Unit == newGuiItem.Unit &&
                                        dbListItem.Volume == newGuiItem.Size)
                                    {
                                        int currentAmount = dbListItem.Amount;
                                        ListItem updatedListItem = new ListItem(((int)newGuiItem.Amount),
                                                               (int)newGuiItem.Size,
                                                               newGuiItem.Unit,
                                                               dbListItem.List,
                                                               dbItem);

                                        _listItemRepository.Delete(dbListItem);
                                        _listItemRepository.Insert(updatedListItem);
                                    }
                                    // Forskellig unit, forskellig Size/volume. Vi tilføjer et det nye ListItem
                                    if (dbItem.ItemId == dbListItem.Item.ItemId &&
                                        dbListItem.List.ListId == currentGuiItemList.ID &&
                                        (dbListItem.Unit != newGuiItem.Unit || dbListItem.Volume != newGuiItem.Size))
                                    {
                                        ListItem updatedListItem = new ListItem(((int)newGuiItem.Amount),
                                                                                   (int)newGuiItem.Size,
                                                                                   newGuiItem.Unit,
                                                                                   dbListItem.List,
                                                                                   dbItem);
                                        _listItemRepository.Insert(updatedListItem);

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

        private bool IsNewItem(GUIItem item)
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
            GUIItemList currentGuiItemList = null;
            foreach (var list in Lists) // Finder den rigtige GuiListe, og sætter vores currentGUiItemList til den
            {
                if (list.Name == CurrentList)
                {
                    currentGuiItemList = list;
                }
            }
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
                        && (uint)dbListItem.Volume == GUIitemToDelete.Size
                        && dbListItem.ListId == currentGuiItemList.ID)
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
            GUIItemList currentGuiItemList = null;
            foreach (var list in Lists) // Finder den rigtige GuiListe, og sætter vores currentGUiItemList til den
            {
                if (list.Name == CurrentList)
                {
                    currentGuiItemList = list;
                }
            }
            using (var uow = Context.CreateUnitOfWork())
            {
                foreach (var dbListItem in _dblistItems)
                {
                    if (dbListItem.Item.ItemName == oldItem.Type && 
                        dbListItem.Unit == oldItem.Unit && 
                        (uint)dbListItem.Volume == oldItem.Size && 
                        dbListItem.Amount == oldItem.Amount &&
                        dbListItem.ListId == currentGuiItemList.ID)
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
                                                                newItem.Unit,
                                                                dbListItem.List,
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
                List<ListItem> skalAltidHave = new List<ListItem>();
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
                            skalAltidHave.Add(dbListItem);
                        }
                    }
                }
                List<ListItem> mangler = new List<ListItem>(skalAltidHave);
                foreach (var STDListItem in skalAltidHave) //Sammenligner de to lister, og ser bort fra det vi har
                {
                    foreach (var ownedListItem in har)
                    {   //Hvis vi har det item vi mangler, retter vi enten amount eller ser bort fra det ListItem
                        if (ownedListItem.Item.ItemName == STDListItem.Item.ItemName &&
                            ownedListItem.Unit == STDListItem.Unit &&
                            ownedListItem.Volume == STDListItem.Volume)
                        {
                            if (ownedListItem.Amount >= STDListItem.Amount)
                            {
                            mangler.Remove(STDListItem);    
                            }
                            if (ownedListItem.Amount <= STDListItem.Amount)
                            {
                                STDListItem.Amount -= ownedListItem.Amount;
                            }
                        }
                    }
                }
                ObservableCollection<GUIItem> ItemsToAdd = new ObservableCollection<GUIItem>();
                foreach (var STDListItem in mangler)
                {
                    GUIItem ItemToAdd = new GUIItem(STDListItem.Item.ItemName,
                                                    (uint)STDListItem.Amount,
                                                    (uint) STDListItem.Volume,
                                                    STDListItem.Unit);
                    ItemsToAdd.Add(ItemToAdd);
                }
                    CurrentList = "Indkøbsliste"; //Sætter at den liste vi skal til at tilføje til hedder Indkøbsliste
                    AddItemsToTable("Indkøbsliste", ItemsToAdd);
                    CurrentList = ListWithNewItem; //Sætter at den liste vi arbejder på, er den liste vi tilføjede til
            }
            else { throw new Exception("List not recognized");}
        }
    }
}
