using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using SmartFridge_WebApplication.Controllers;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using List = NUnit.Framework.List;
using SmartFridge_Cache;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    class EditItemControllerUnitTests
    {

        private EditItemController uut;
        [SetUp]
        public void Setup()
        {
            uut = new EditItemController();
            var dalfacade = Substitute.For<ISmartFridgeDALFacade>();
            Cache.DalFacade = dalfacade;
            SmartFridge_WebModels.List currentList = new SmartFridge_WebModels.List("TestList"){ListId = 0};
            Cache.CurrentList = currentList;
            Item item = new Item("test") {ItemId = 1};
            ListItem listitem = new ListItem(1, 1, "l", default(DateTime), null, item){ItemId = item.ItemId};
            Cache.CurrentListItems.Add(listitem);
            Cache.DbItems.Add(item);
        }

        [Test]
        public void EditView()
        {
            var actResult = uut.EditItem(new GUIItem()) as ViewResult;

            Assert.That(actResult.ViewName, Is.EqualTo(""));    
        }

        [Test]
        public void SelectedItem_ItemFound()
        {
            EditItemController.selectedUnit(new GUIItem("TestGUIItem", 1, 1, "l"));
        }

        [Test]
        public void SelectedItem_ItemNotFound()
        {
            EditItemController.selectedUnit(new GUIItem("TestGUIItem", 1, 1, "F"));
        }

        [Test]
        public void UpdateItem_Item_EverythingButTypeAndNoShelfLifeUpdated()
        {
            uut.EditItem(new GUIItem("test", 1, 1, "l"));
            FormCollection collection = new FormCollection();
            collection.Add("Type","test");
            collection.Add("Amount", "3");
            collection.Add("Volume", "2");
            collection.Add("units", "kg");
            collection.Add("date", "");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));                
        }

        [Test]
        public void UpdateItem_Item_EverythingButTypeUpdated()
        {
            uut.EditItem(new GUIItem("test", 1, 1, "l"){ShelfLife = default(DateTime)});
            FormCollection collection = new FormCollection();
            collection.Add("Type", "test");
            collection.Add("Amount", "3");
            collection.Add("Volume", "2");
            collection.Add("units", "kg");
            collection.Add("date", "26-05-2015");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

        [Test]
        public void UpdateItem_OnlyTypeUpdatedExistingListItem()
        {
            Cache.DbItems.Add(new Item("UpdatedTest") { ItemId = 2 });
            Cache.CurrentListItems.Add(new ListItem(2,3,"kg", new DateTime(2015,05,26)){ItemId = 2, Item = new Item(){ItemId = 2}});
            uut.EditItem(new GUIItem("test", 1, 1, "l") { ShelfLife = default(DateTime) });
            FormCollection collection = new FormCollection();
            collection.Add("Type", "UpdatedTest");
            collection.Add("Amount", "2");
            collection.Add("Volume", "3");
            collection.Add("units", "kg");
            collection.Add("date", "26-05-2015");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

        [Test]
        public void UpdateItem_OnlyTypeUpdatedNotExistingListItem()
        {
            Cache.DbItems.Add(new Item("UpdatedTest") { ItemId = 2 });
            uut.EditItem(new GUIItem("test", 1, 1, "l") { ShelfLife = default(DateTime) });
            FormCollection collection = new FormCollection();
            collection.Add("Type", "UpdatedTest");
            collection.Add("Amount", "1");
            collection.Add("Volume", "1");
            collection.Add("units", "l");
            collection.Add("date", "01-01-0001");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

        [Test]
        public void UpdateItem_OnlyTypeUpdatedNonExistantItem()
        {
            uut.EditItem(new GUIItem("test", 1, 1, "l") { ShelfLife = default(DateTime) });
            FormCollection collection = new FormCollection();
            collection.Add("Type", "UpdatedTest");
            collection.Add("Amount", "1");
            collection.Add("Volume", "1");
            collection.Add("units", "l");
            collection.Add("date", "01-01-0001");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

        [Test]
        public void UpdateItem_NoListItemsToUpdate()
        {
            Cache.CurrentList = new SmartFridge_WebModels.List();
            Cache.CurrentListItems.Add(new ListItem(){Item = new Item(){ItemId = -1}});
            FormCollection collection = new FormCollection();
            collection.Add("Type", "UpdatedTest");
            collection.Add("Amount", "1");
            collection.Add("Volume", "1");
            collection.Add("units", "l");
            collection.Add("date", "01-01-0001");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

        [Test]
        public void UpdateItem_NoDbItems()
        {
            Cache.CurrentList = new SmartFridge_WebModels.List();
            Cache.DbItems.Add(new Item());
            uut.EditItem(new GUIItem("test", 1, 1, "l"));
            FormCollection collection = new FormCollection();
            collection.Add("Type", "test");
            collection.Add("Amount", "3");
            collection.Add("Volume", "2");
            collection.Add("units", "kg");
            collection.Add("date", "");
            var actResult = uut.UpdateItem(collection) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView")); 
        }
  
    }
}

