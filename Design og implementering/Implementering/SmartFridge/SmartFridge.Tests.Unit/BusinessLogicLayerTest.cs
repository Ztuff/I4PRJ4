using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer;
using InterfacesAndDTO;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    public class BusinessLogicLayerTest
    {
        public BLL uut;
        [SetUp]
        public void SetUp()
        {
            //NB: Tests only work if your ListItems and Items tables are CLEARED!!
            uut = new BLL();
            uut.CurrentList = "Køleskab"; //High coupling...
            var items = uut.WatchItems;
        }

        [Test]
        public void WatchItems_Add2ItemsToFridge_2ItemsReturned()
        {
            var items1 = uut.WatchItems;
            Assert.AreEqual(0, items1.Count);
            var items2 = new ObservableCollection<GUIItem>();
            var rnd = new Random();

            for (int i = 0; i < 2; i++)
            {
                string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
                items2.Add(new GUIItem(type, 5, 1, "Unit") { ShelfLife = DateTime.Now });
            }
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items2);

            var items3 = uut.WatchItems;

            Assert.AreEqual(2, items3.Count);

            foreach (var guiItem in items2)
            {
                uut.DeleteItem(guiItem);
                uut.DeleteItemWithType(guiItem.Type);
            }

        }

        [Test]
        public void CreateNewItem_5ParametersInserted_Same5ParametersInNewItem()
        {
            string type = "type";
            uint amount = 1;
            uint size = 1;
            string unit = "unit";
            DateTime year3000 = new DateTime(3000, 1, 1);

            var item = uut.CreateNewItem(type, amount, size, unit, year3000);
            Assert.AreEqual(type, item.Type);
            Assert.AreEqual(amount, item.Amount);
            Assert.AreEqual(size, item.Size);
            Assert.AreEqual(unit, item.Unit);
            Assert.AreEqual(year3000, item.ShelfLife.Date);
        }

        [Test]
        public void CheckShelfLife_NoExpiredItems_NoNotifications()
        {
            var GUIItemlist = new GUIItemList(5, "TestList");
            GUIItemlist.ItemList.Add(new GUIItem("Test", 1, 1, "1") { ShelfLife = new DateTime(3000, 1, 1) });
            GUIItemlist.ItemList.Add(new GUIItem("Test", 1, 1, "1") { ShelfLife = new DateTime(3000, 1, 1) });

            var notifications = uut.CheckShelfLife(GUIItemlist);

            Assert.AreEqual(0, notifications.Count);

        }

        [Test]
        public void CheckShelfLife_2ExperiedItems_2Notifications()
        {
            var GUIItemlist = new GUIItemList(5, "TestList");
            GUIItemlist.ItemList.Add(new GUIItem("Test", 1, 1, "1") { ShelfLife = new DateTime(1993, 1, 1) });
            GUIItemlist.ItemList.Add(new GUIItem("Test", 1, 1, "1") { ShelfLife = new DateTime(1984, 1, 1) });

            var notifications = uut.CheckShelfLife(GUIItemlist);

            Assert.AreEqual(2, notifications.Count);


        }

        [Test]
        public void Compare_TwoSimilarItems_ReturnTrue()
        {
            var item1 = new GUIItem("Type", 1, 1, "Unit");
            var item2 = new GUIItem("Type", 1, 1, "Unit");

            var equality = uut.Compare(item1, item2);

            Assert.AreEqual(true, equality);
        }

        [Test]
        public void Compare_TwoNonSimilarItems_ReturnFalse()
        {
            var item1 = new GUIItem("Type", 1, 1, "Unit");
            var item2 = new GUIItem("Type 2", 1, 1, "Unit");

            var equality = uut.Compare(item1, item2);

            Assert.AreEqual(false, equality);
        }

        [Test]
        public void AddItemsToTable_1NewItem_SameItemAddedToKøleskab()
        {
            var items = new ObservableCollection<GUIItem>();
            var rnd = new Random();
            string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
            items.Add(new GUIItem(type, 5, 1, "Unit") { ShelfLife = DateTime.Now });
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items);

            //Find the list...
            GUIItemList list = null;

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;
            foreach (var guiItem1 in items)
            {
                foreach (var guiItem2 in list.ItemList)
                {
                    if (uut.Compare(guiItem1, guiItem2))
                        EqualItems++;
                }
            }

            Assert.AreEqual(1, EqualItems);

            foreach (var guiItem in items)
            {
                uut.DeleteItem(guiItem);
                uut.DeleteItemWithType(guiItem.Type);
            }
        }

        [Test]
        public void AddItemsToTable_2NewItems_SameItemAddedsToKøleskab()
        {
            var items = new ObservableCollection<GUIItem>();
            var rnd = new Random();

            for (int i = 0; i < 2; i++)
            {
                string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
                items.Add(new GUIItem(type, 5, 1, "Unit") { ShelfLife = DateTime.Now });
            }
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items);

            //Find the list...
            GUIItemList list = null;

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;

            foreach (var guiItem1 in items)
            {
                foreach (var guiItem2 in list.ItemList)
                {
                    if (uut.Compare(guiItem1, guiItem2))
                        EqualItems++;
                }
            }

            Assert.AreEqual(2, EqualItems);

            foreach (var guiItem in items)
            {
                uut.DeleteItem(guiItem);
                uut.DeleteItemWithType(guiItem.Type);
            }
        }

        [Test]
        public void AddItemsToTable_AddTwoSimilarItems_ItemsAreStillSeperateInKøleskab()
        {
            var items1 = new ObservableCollection<GUIItem>();
            var items2 = new ObservableCollection<GUIItem>();
            var rnd = new Random();
            string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
            var ShelfLife = DateTime.Now;
            string listName = "Køleskab";
            items1.Add(new GUIItem(type, 1, 1, "l") { ShelfLife = ShelfLife });
            items2.Add(new GUIItem(type, 1, 2, "kg") { ShelfLife = ShelfLife });
            uut.AddItemsToTable(listName, items1);
            uut.AddItemsToTable(listName, items2);

            //Find the list...
            GUIItemList list = null;

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;
            
            foreach (var guiItem2 in list.ItemList)
            {
                if (uut.Compare(items1[0], guiItem2))
                    EqualItems++;
                if (uut.Compare(items2[0], guiItem2))
                    EqualItems++;
            }

            Assert.AreEqual(2, EqualItems);

            uut.DeleteItem(items1[0]);
            uut.DeleteItem(items2[0]);
            uut.DeleteItemWithType(items1[0].Type);
        }

        [Test]
        public void AddItemsToTable_AddTwoEqualItems_ItemsAreAddedTogetherAsOneInKøleskab()
        {
            var items1 = new ObservableCollection<GUIItem>();
            var items2 = new ObservableCollection<GUIItem>();
            var rnd = new Random();
            string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
            var ShelfLife = DateTime.Now;
            string listName = "Køleskab";
            items1.Add(new GUIItem(type, 1, 1, "l") { ShelfLife = ShelfLife });
            items2.Add(new GUIItem(type, 1, 1, "l") { ShelfLife = ShelfLife });
            uut.AddItemsToTable(listName, items1);
            uut.AddItemsToTable(listName, items2);

            //Find the list...
            GUIItemList list = null;

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;
            var itemToLookFor = new GUIItem(type, 2, 1, "l") { ShelfLife = ShelfLife };

            foreach (var guiItem2 in list.ItemList)
            {
                if (uut.Compare(itemToLookFor, guiItem2))
                    EqualItems++;
            }

            Assert.AreEqual(1, EqualItems);

            uut.DeleteItem(itemToLookFor);
            uut.DeleteItemWithType(itemToLookFor.Type);
        }

        [Test]
        public void ChangeItem_ChangeItemFromMilkToCola_ItemChanged()
        {
            //We first have to add an item, so that we have something to change.
            var items = new ObservableCollection<GUIItem>();
            items.Add(new GUIItem("Milk", 1, 1, "l"){ShelfLife = DateTime.Now.Date});
            string listName = "Køleskab";

            uut.AddItemsToTable(listName, items);

            GUIItemList list = null;
            var cola = new GUIItem("Cola", 1, 1, "l") {ShelfLife = DateTime.Now.Date};
            uut.ChangeItem(items[0], cola);

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;
            foreach (var listItem in list.ItemList)
            {
                if (uut.Compare(listItem, cola))
                    EqualItems++;
            }
            Assert.AreEqual(1, EqualItems);

            uut.DeleteItem(cola);
            uut.DeleteItemWithType("Cola");
            uut.DeleteItemWithType("Milk");
        }

        [Test]
        public void ChangeItem_ChangeItemFromMilkToCola_ItemChangedAndAddedToExistingCola()
        {
            //We first have to add an item, so that we have something to change.
            var items = new ObservableCollection<GUIItem>();
            items.Add(new GUIItem("Milk", 1, 1, "l") { ShelfLife = DateTime.Now.Date });
            items.Add(new GUIItem("Cola", 1, 1, "l") { ShelfLife = DateTime.Now.Date });
            string listName = "Køleskab";

            uut.AddItemsToTable(listName, items);

            GUIItemList list = null;
            var cola = new GUIItem("Cola", 1, 1, "l") { ShelfLife = DateTime.Now.Date };
            uut.ChangeItem(items[0], cola);

            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Køleskab"))
                    list = guiItemList;
            }
            int EqualItems = 0;
            var ItemToLookFor = new GUIItem("Cola", 2, 1, "l") {ShelfLife = DateTime.Now.Date};
            foreach (var listItem in list.ItemList)
            {
                if (uut.Compare(listItem, ItemToLookFor))
                    EqualItems++;
            }
            Assert.AreEqual(1, EqualItems);

            uut.DeleteItem(ItemToLookFor);
            uut.DeleteItemWithType("Cola");
            uut.DeleteItemWithType("Milk");
        }

        [Test]
        public void STDToShopListControl_MilkOnSTDListButNotInFridge_MilkInShoppingList()
        {
            uut.CurrentList = "Standard-beholdning";
            //Adding the milk to the STDlist...
            var items = new ObservableCollection<GUIItem>();
            items.Add(new GUIItem("Milk", 1, 1, "l") { ShelfLife = DateTime.Now.Date });
            string listName = "Standard-beholdning";
            //Checking if the fridge has a milk...
            uut.AddItemsToTable(listName, items);

            //Get the shopping list
            GUIItemList list = null;
            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Indkøbsliste"))
                    list = guiItemList;
            }

            //See if the milk is in the shopping list
            int EqualItems = 0;
            foreach (var listItem in list.ItemList)
            {
                if (uut.Compare(listItem, items[0]))
                    EqualItems++;
            }

            Assert.AreEqual(1, EqualItems);
            uut.DeleteItem(items[0]);
            uut.CurrentList = "Indkøbsliste";
            uut.DeleteItem(items[0]);
            uut.DeleteItemWithType("Milk");
        }

        [Test]
        public void STDToShopListControl_MilkOnSTDListAndInFridge_NothingChanged()
        {
           
            uut.CurrentList = "Køleskab";
            //Adding the milk to the Fridge...
            var items2 = new ObservableCollection<GUIItem>();
            items2.Add(new GUIItem("Milk", 1, 1, "l") { ShelfLife = DateTime.Now.Date });
            string listName2 = "Køleskab";
            //Checking if the fridge has a milk...
            uut.AddItemsToTable(listName2, items2);

            uut.CurrentList = "Standard-beholdning";
            //Adding the milk to the STDlist...
            var items = new ObservableCollection<GUIItem>();
            items.Add(new GUIItem("Milk", 1, 1, "l") { ShelfLife = DateTime.Now.Date });
            string listName = "Standard-beholdning";
            //Checking if the fridge has a milk...
            uut.AddItemsToTable(listName, items);

            //Get the shopping list
            GUIItemList list = null;
            foreach (var guiItemList in uut.Lists)
            {
                if (guiItemList.Name.Equals("Indkøbsliste"))
                    list = guiItemList;
            }

            //See if the milk is in the shopping list
            int EqualItems = 0;
            foreach (var listItem in list.ItemList)
            {
                if (uut.Compare(listItem, items[0]))
                    EqualItems++;
            }

            Assert.AreEqual(0, EqualItems);
            uut.DeleteItem(items[0]);
            uut.CurrentList = "Standard-beholdning";
            uut.DeleteItem(items[0]);
            uut.DeleteItemWithType("Milk");
        }

        [Test]
        public void STDToShopListControl_UnrecognizableListInserted_ExceptionThrown()
        {
            Assert.Throws<Exception>(() => uut.STDToShopListControl("RandomList"));
        }

        [Test]
        public void Types_3ItemsInItemsList_3ItemsReturned()
        {
            var items = new ObservableCollection<GUIItem>();
            var rnd = new Random();

            for (int i = 0; i < 3; i++)
            {
                string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
                items.Add(new GUIItem(type, 5, 1, "Unit") { ShelfLife = DateTime.Now });
            }
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items);

            var types = uut.Types;
            int EqualItems = 0;
            foreach (var guiItem in types)
            {
                foreach (var item in items)
                {
                    if (guiItem.Type.Equals(item.Type))
                        EqualItems++;
                }
            }

            Assert.AreEqual(3, EqualItems);

            foreach (var guiItem in items)
            {
                uut.DeleteItem(guiItem);
            }
            foreach (var guiItem in types)
            {
                uut.DeleteItemWithType(guiItem.Type);
            }
        }

        [Test]
        public void GetList_RequestFridge_FridgeReturned()
        {
            var list = uut.GetList("Køleskab");
            Assert.AreEqual("Køleskab", list.Name);
        }

        [Test]
        public void GetList_RequestUnknown_NullReturned()
        {
            var list = uut.GetList("Unknown");
            Assert.AreEqual(null, list);
        }
    }
}
