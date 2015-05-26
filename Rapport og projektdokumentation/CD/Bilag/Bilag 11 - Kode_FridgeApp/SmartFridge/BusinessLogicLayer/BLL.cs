using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    /// Binds user interaction logic with DAL logic
    /// </summary>
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

        /// <summary>
        /// Sets context and repositories
        /// </summary>
        public BLL()
        {
            Context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(Context);
            _listRepository = new ListRepository(Context);
            _listItemRepository = new ListItemRepository(Context);
        }
        /// <summary>
        /// Deletes item from db based on its name
        /// </summary>
        /// <param name="type"></param>
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
        /// <summary>
        /// Units
        /// </summary>
        public readonly ObservableCollection<string> UnitNames = new ObservableCollection<string>()
        {
            "l",
            "dl",
            "cl",
            "ml",
            "kg",
            "g"
        };
        /// <summary>
        /// Types
        /// </summary>
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
        /// <summary>
        /// Creates list of GUIItems shown
        /// </summary>
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

                //If the list doesn't exist:
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
                                guiItem.ShelfLife = dbListItem.ShelfLife;
                                guiItems.Add(guiItem);
                            }
                        }
                    }


                }

                return guiItems;
            }
        }
        /// <summary>
        /// Creates list in db
        /// </summary>
        [ExcludeFromCodeCoverage]
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
        /// <summary>
        /// Loads all data from db into class attributes
        /// </summary>
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
                                    Unit = listItem.Unit,
                                    ID = listItem.ItemId,
                                    ShelfLife = listItem.ShelfLife

                                };
                                list.ItemList.Add(guiItem);
                                break;
                            }
                        }
                }
            }

        }
        /// <summary>
        /// Returns GUIItemList based on listname
        /// </summary>
        /// <param name="ListName"></param>
        /// <returns></returns>
        public GUIItemList GetList(string ListName)
        {
            foreach (var guiItemList in Lists)
            {
                if (guiItemList.Name.Equals(ListName))
                    return guiItemList;
            }
            return null;
        }
        /// <summary>
        /// Controls items shelf-life and returns list with notifications for old items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<Notification> CheckShelfLife(GUIItemList items)
        {
            var list = new List<Notification>();
            foreach (var item in items.ItemList)
            {
                if (item.ShelfLife != null)
                {
                    if (item.ShelfLife.Date <= DateTime.Now)
                    { //The item is seen as 'expired' when we're on the same day as its expiration date
                        string message = item.Type + " blev for gammel d. " + DateTime.Now.Date;
                        list.Add(new Notification(message, DateTime.Now, item.ID));
                    }
                }

            }
            return list;
        }

        /// <summary>
        /// Compares 2 GuiItems
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates new item based on parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <param name="size"></param>
        /// <param name="unit"></param>
        /// <param name="shelfLife"></param>
        /// <returns></returns>
        public GUIItem CreateNewItem(string type, uint amount, uint size, string unit, DateTime shelfLife)
        {
            GUIItem item = new GUIItem();
            item.Type = type;
            item.Amount = amount;
            item.Size = size;
            item.Unit = unit;
            item.ShelfLife = shelfLife;
            return item;
        }
        /// <summary>
        /// Maps GUIItems to db item, ListItem and list. If specific GUIItem exists it corrects the amount. else
        /// it adds the corresponding to the db
        /// </summary>
        /// <param name="currentListName"></param>
        /// <param name="newGuiItems"></param>
        public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> newGuiItems)
        {

            GUIItemList currentGuiItemList = null;
            if (Lists == null) //Hvis den liste vi vil tilføje items til ikke findes, opretter vi den OBS: currentList er den lokale, CurrentList er klassens 
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
                                    Volume = (int)newGuiItem.Size,
                                    ShelfLife = newGuiItem.ShelfLife

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
                                        ListItem updatedListItem = new ListItem(((int)newGuiItem.Amount + currentAmount),
                                                               (int)newGuiItem.Size,
                                                               newGuiItem.Unit,
                                                               dbListItem.List,
                                                               dbItem,
                                                               newGuiItem.ShelfLife);

                                        _listItemRepository.Delete(dbListItem);
                                        _listItemRepository.Insert(updatedListItem);
                                        break;
                                    }
                                    // Forskellig unit, forskellig Size/volume, forskellig shelflife - Vi tilføjer et det nye ListItem
                                    if (dbItem.ItemId == dbListItem.Item.ItemId &&
                                        dbListItem.List.ListId == currentGuiItemList.ID &&
                                        (dbListItem.Unit != newGuiItem.Unit || dbListItem.Volume != newGuiItem.Size || dbListItem.ShelfLife != newGuiItem.ShelfLife))
                                    {
                                        ListItem updatedListItem = new ListItem(((int)newGuiItem.Amount),
                                                                                   (int)newGuiItem.Size,
                                                                                   newGuiItem.Unit,
                                                                                   dbListItem.List,
                                                                                   dbItem,
                                                                                   newGuiItem.ShelfLife);
                                        _listItemRepository.Insert(updatedListItem);
                                        break;

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
        /// <summary>
        /// Controls the GUIItem Type to see if such and Item exists in db
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsNewItem(GUIItem item)
        {
            foreach (var dbItem in _dbItems)
            {
                if (item.Type == dbItem.ItemName)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Deletes corresponding databse elements based on the GUIItem
        /// </summary>
        /// <param name="GUIitemToDelete"></param>
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
        /// <summary>
        /// Deletes old Item and adds new item with updated values
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
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
                    //Find den item vi gerne vil lave om
                    if (dbListItem.Item.ItemName == oldItem.Type &&
                        dbListItem.Unit == oldItem.Unit &&
                        (uint)dbListItem.Volume == oldItem.Size &&
                        dbListItem.Amount == oldItem.Amount &&
                        dbListItem.ListId == currentGuiItemList.ID)
                    {
                        ListItem updatedListItem = new ListItem((int)newItem.Amount,
                            (int)newItem.Size,
                            newItem.Unit,
                            dbListItem.List,
                            dbListItem.Item,
                            newItem.ShelfLife);

                        if (oldItem.Type != newItem.Type) //If they're not the same type of item...
                        {

                            bool itemIsNew = false;
                            if (IsNewItem(newItem)) //Check if it's a new 'Item'
                            {
                                itemIsNew = true;
                                var dbItem = new Item(newItem.Type, (int)newItem.Size, newItem.Unit);
                                _itemRepository.Insert(dbItem);
                                updatedListItem.Item = dbItem;
                                //The item repository doesn't update until 'uow.SaveChanges' is run, so we'll have to set it here
                            }
                            if (!itemIsNew)
                            {
                                foreach (var dbItem in _dbItems) //If it's not a entirely new item, we'll use this
                                {
                                    if (dbItem.ItemName.Equals(newItem.Type))
                                    {
                                        updatedListItem.Item = dbItem;
                                    }
                                }
                            }

                            //Now let's see if we can add it to an already existing item on this list...
                            bool itemAddedToExistingItem = false;
                            foreach (var dblistitem2 in _dblistItems)
                            {
                                if (dblistitem2.Item.ItemName == newItem.Type &&
                                    dblistitem2.Unit == newItem.Unit &&
                                    (uint)dblistitem2.Volume == newItem.Size &&
                                    dblistitem2.ListId == currentGuiItemList.ID)
                                {
                                    //If an equal item already exists, just add the amount to it
                                    _listItemRepository.Delete(dbListItem);  // combining two items into one means deleting both of them...
                                    _listItemRepository.Delete(dblistitem2);
                                    var test = dblistitem2.Amount;
                                    dblistitem2.Amount = dblistitem2.Amount + (int)newItem.Amount;
                                    _listItemRepository.Insert(dblistitem2); //...and adding one new
                                    uow.SaveChanges();
                                    itemAddedToExistingItem = true;
                                    LoadFromDB();
                                    break;
                                }
                            }


                            if (!itemAddedToExistingItem)
                            {
                                _listItemRepository.Delete(dbListItem);
                                _listItemRepository.Insert(updatedListItem);
                                uow.SaveChanges();
                                LoadFromDB();
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If new items added to STDList, it compares STDList to Fridge and adds the difference to Shop-List
        /// </summary>
        /// <param name="ListWithNewItem"></param>
        public void STDToShopListControl(string ListWithNewItem)
        {
            if (ListWithNewItem == "Køleskab" || ListWithNewItem == "Indkøbsliste") { return; }
            if (ListWithNewItem == "Standard-beholdning")
            {
                List<ListItem> fridge = new List<ListItem>();
                List<ListItem> standardCapacity = new List<ListItem>();
                //Kan ikke gå fra list til vores listItem.
                //Bliver nødt til at lede alle listItems igennem og finde dem der tilhører
                //den rigtige liste.

                using (var uow = Context.CreateUnitOfWork())
                {
                    foreach (var dbListItem in _dblistItems) //Laver en liste med hvad vi har, og hvad der skal være
                    {
                        if (dbListItem.List.ListName == "Køleskab")
                        {
                            fridge.Add(dbListItem);
                        }
                        else if (dbListItem.List.ListName == "Standard-beholdning")
                        {
                            standardCapacity.Add(dbListItem);
                        }
                    }
                }
                List<ListItem> difference = new List<ListItem>(standardCapacity);
                foreach (var STDListItem in standardCapacity) //Sammenligner de to lister, og ser bort fra det vi har
                {
                    foreach (var ownedListItem in fridge)
                    {   //Hvis vi har det item vi mangler, retter vi enten amount eller ser bort fra det ListItem
                        if (ownedListItem.Item.ItemName == STDListItem.Item.ItemName &&
                            ownedListItem.Unit == STDListItem.Unit &&
                            ownedListItem.Volume == STDListItem.Volume)
                        {
                            if (ownedListItem.Amount >= STDListItem.Amount)
                            {
                                difference.Remove(STDListItem);
                            }
                            if (ownedListItem.Amount <= STDListItem.Amount)
                            {
                                STDListItem.Amount -= ownedListItem.Amount;
                            }
                        }
                    }
                }
                ObservableCollection<GUIItem> ItemsToAdd = new ObservableCollection<GUIItem>();
                foreach (var STDListItem in difference)
                {
                    GUIItem ItemToAdd = new GUIItem(STDListItem.Item.ItemName,
                                                    (uint)STDListItem.Amount,
                                                    (uint)STDListItem.Volume,
                                                    STDListItem.Unit){ShelfLife = DateTime.MaxValue};
                    ItemsToAdd.Add(ItemToAdd);
                }
                CurrentList = "Indkøbsliste"; //Sætter at den liste vi skal til at tilføje til hedder Indkøbsliste
                AddItemsToTable("Indkøbsliste", ItemsToAdd);
                CurrentList = ListWithNewItem; //Sætter at den liste vi arbejder på, er den liste vi tilføjede til
            }
            else { throw new Exception("List not recognized"); }
        }
    }
}
