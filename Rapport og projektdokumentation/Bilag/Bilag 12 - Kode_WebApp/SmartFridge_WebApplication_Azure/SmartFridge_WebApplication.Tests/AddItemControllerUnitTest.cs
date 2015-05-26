using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using SmartFridge_Cache;
using SmartFridge_WebApplication.Controllers;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using List = NUnit.Framework.List;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    public class AddItemControllerUnitTest
    {
        private AddItemController uut;
        private ISmartFridgeDALFacade dalfacade = Substitute.For<ISmartFridgeDALFacade>();
        ListItem globalTeslListItem = new ListItem();
        [SetUp]
        public void Setup()
        {
            uut = new AddItemController();
            
            Cache.DalFacade = dalfacade;
            var currentList = new SmartFridge_WebModels.List("TestList") { ListId = 0 };
            Cache.CurrentList = currentList;
            Item item = new Item("TestType") { ItemId = 1 };
            ListItem listitem = new ListItem(1, 1, "l", default(DateTime), null, item) { ItemId = item.ItemId };
            dalfacade.GetUnitOfWork().ListRepo.GetAll().Returns(new List<SmartFridge_WebModels.List>() { currentList });
            dalfacade.GetUnitOfWork().ItemRepo.GetAll().Returns(new List<Item> { item });
            dalfacade.GetUnitOfWork().ListItemRepo.GetAll().Returns(new List<ListItem>() { listitem });
            globalTeslListItem = listitem;
        }

        [Test]
        public void AddItem_TestLoadingCorrectModel_ReturnCorrectModel()
        {

            var actResult = uut.AddItem() as ViewResult;
            Assert.That(actResult.ViewName, Is.EqualTo(""));
        }

        [Test]
        public void addNewItem_Add1ItemAndDontExit_AddItemReturned()
        {
            var result = uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "Add") as PartialViewResult;
            Assert.That(result.ViewName, Is.EqualTo("~/Views/AddItem/AddItem.cshtml"));
        }

        [Test]
        public void addNewItem_Add2ItemsAndMergeAmount_AddItemReturned()
        {
            uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "Add");
            var result = uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "Add") as PartialViewResult;
            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        public void addNewItem_Add2ItemsAndDontMergeAmount_AddItemReturned()
        {
            uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "Add");
            var result = uut.addNewItem("TestItem2", "2", "2", "2", "04-06-1993", "Add") as PartialViewResult;
            Assert.That(result.ViewName, Is.EqualTo("~/Views/AddItem/AddItem.cshtml"));
        }

        [Test]
        public void addNewItem_Add1ItemAndExit_LisViewReturned()
        {

            RedirectToRouteResult result = uut.addNewItem("TestItem", "1", "1", "l", "", "Exit") as RedirectToRouteResult;
            Assert.That(result.RouteValues["action"], Is.EqualTo("ListView"));

        }

        [Test]
        public void Exit_NoNewItems_ReturnNull()
        {
            var result = uut.Exit() as RedirectToRouteResult;
            Assert.AreEqual(result, null);
        }

        [Test]
        public void Exit_NewItemAlreadyExistsMergeAmount()
        {
            dalfacade.GetUnitOfWork().ListItemRepo.Find(null).ReturnsForAnyArgs(globalTeslListItem);
            var result = uut.addNewItem("TestItem", "1", "1", "l", "", "Exit") as RedirectToRouteResult;
            Assert.That(result.RouteValues["action"], Is.EqualTo("ListView"));
        }
    }
}
