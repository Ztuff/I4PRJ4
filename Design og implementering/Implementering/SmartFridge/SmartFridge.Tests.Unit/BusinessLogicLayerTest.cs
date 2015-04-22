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
            uut = new BLL();
        }

        [Test]
        public void CreateNewItem_4ParametersInserted_Same4ParametersInNewItem()
        {
            string type = "type";
            uint amount = 1;
            uint size = 1;
            string unit = "unit";

            var item = uut.CreateNewItem(type, amount, size, unit);
            Assert.AreEqual(type, item.Type);
            Assert.AreEqual(amount, item.Amount);
            Assert.AreEqual(size, item.Size);
            Assert.AreEqual(unit, item.Unit);
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
        //Integrationstest
        //Forudsætninger: 
        // - At der er en database
        // - At der er en liste der hedder 'Køleskab'
        public void AddItemsToTable_1NewItem_SameItemAddedToKøleskab()
        {
            //public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> newItems)
            var items = new ObservableCollection<GUIItem>();
            var rnd = new Random();
            string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
            items.Add(new GUIItem(type, 5, 1, "Unit"));
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

            Assert.AreEqual(1,EqualItems);

            foreach (var guiItem in items)
            {
                uut.DeleteItem(guiItem);
            }
        }

        [Test]
        //Integrationstest
        //Forudsætninger: 
        // - At der er en database
        // - At der er en liste der hedder 'Køleskab'
        public void AddItemsToTable_2NewItems_SameItemAddedsToKøleskab()
        {
            //public void AddItemsToTable(string currentListName, ObservableCollection<GUIItem> newItems)
            var items = new ObservableCollection<GUIItem>();
            var rnd = new Random();

            for (int i = 0; i < 2; i++)
            {
                string type = rnd.Next(int.MinValue, int.MaxValue).ToString();
                items.Add(new GUIItem(type, 5, 1, "Unit"));
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
            }
        }

    }
}
