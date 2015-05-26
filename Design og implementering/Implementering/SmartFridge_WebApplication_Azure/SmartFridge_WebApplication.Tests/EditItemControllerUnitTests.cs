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
            Item item = new Item("TestType") {ItemId = 1};
            ListItem listitem = new ListItem(1, 1, "l", default(DateTime), null, item){ItemId = item.ItemId};
            Cache.DbItems.Add(item);
            dalfacade.GetUnitOfWork().ListItemRepo.GetAll().Returns(new List<ListItem>() {listitem});
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
        public void UpdateItem_Item_ItemUpdated()
        {
            var actResult = uut.EditItem(new GUIItem("test", 1, 1, "l"));
                        _updatedGUIItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["units"]
                );
            FormCollection collection = new FormCollection();
            collection.Add(new NameValueCollection(){"Type",});
            uut.UpdateItem()
        }
  
    }
}

