using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer;
using InterfacesAndDTO;
using NUnit.Framework;
using UserControlLibrary;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    class CtrlItemListTest
    {
        public BLL uut;

        [SetUp]
        public void SetUp()
        {
            uut = new BLL();
        }

/*        [Test]  //Tester om item.Amount bliver ændret !OBS! Udkommenteret indtil ChangeItem Virker!
        public void Compare_ItemAmountChanged_ReturnTrue()
        {
            //Opretter Items
            var guiItemToAdd = new GUIItem("TestItem", 2, 1, "UNIT");
            var guiItemChangeTo = new GUIItem("TestItem", 3, 1, "UNIT");

            var items = new ObservableCollection<GUIItem>();
            items.Add(guiItemToAdd);
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items);

            //Change virker ikke?
            //uut.ChangeItem(guiItemToAdd, guiItemChangeTo);

            bool test = false;

            //Leder efter item
            foreach (var content in uut.WatchItems)
            {
                if (content.Type == guiItemToAdd.Type && content.Size == guiItemToAdd.Size && content.Unit == guiItemToAdd.Unit && content.Amount != guiItemToAdd.Amount && content.Amount == guiItemChangeTo.Amount)
                {
                    test = true; //True hvis: Alt passer undtaget "Amount". Og guiItemChangeTo.Amount = content.Amount
                }
            }
            Assert.AreEqual(true, test);

        }*/

        [Test]
        public void Delete_ItemWasDeleted_ReturnFalse()
        {
            //vælger liste
            uut.CurrentList = "Køleskab";
            //Opretter Item
            var guiItemToDelete = new GUIItem("TestDeleteItem", 1, 1, "l");

            //Indsætter nyt item -> forudsætter at "addItemsToTable()"-funktionen virker
            var items = new ObservableCollection<GUIItem>();
            items.Add(guiItemToDelete);
            string listName = "Køleskab";
            uut.AddItemsToTable(listName, items);


            //Henter nyeste data fra DB
            uut.LoadFromDB();
            //Sletter item
            uut.DeleteItem(guiItemToDelete);
            //Henter nyeste data fra DB
            uut.LoadFromDB();

            bool test = false;

            //Leder efter item
            foreach (var content in uut.WatchItems)
            {
                if (content.Type == guiItemToDelete.Type && content.Size == guiItemToDelete.Size && content.Unit == guiItemToDelete.Unit && content.Amount == guiItemToDelete.Amount)
                {
                    test = true; //bliver sat til true hvis et variable matcher den på listen
                }
            }
            Assert.AreEqual(false, test);



            
        }

    }
}
