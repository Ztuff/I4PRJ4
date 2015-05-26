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
        [SetUp]
        public void Setup()
        {
            uut = new AddItemController();
            var dalfacade = Substitute.For<ISmartFridgeDALFacade>();
            Cache.DalFacade = dalfacade;
            var currentList = new SmartFridge_WebModels.List("TestList") { ListId = 0 };
            Cache.CurrentList = currentList;
            Item item = new Item("TestType") { ItemId = 1 };
            ListItem listitem = new ListItem(1, 1, "l", default(DateTime), null, item) { ItemId = item.ItemId };
            dalfacade.GetUnitOfWork().ListRepo.GetAll().Returns(new List<SmartFridge_WebModels.List>() { currentList });
            dalfacade.GetUnitOfWork().ItemRepo.GetAll().Returns(new List<Item> { item });
            dalfacade.GetUnitOfWork().ListItemRepo.GetAll().Returns(new List<ListItem>() { listitem });
        }

        [Test]
        public void addNewItem_Add1ItemAndExit_LisViewReturned()
        {
            RedirectToRouteResult result = uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "Exit") as RedirectToRouteResult;
            Assert.That(result.RouteValues.Values.ElementAt(1), Is.EqualTo("LisView"));
        }

        [Test]
        public void addNewItem_Add1ItemAndDontExit_AddItemReturned()
        {
            var result = uut.addNewItem("TestItem", "1", "1", "l", "06-09-1993", "no") as PartialViewResult;
            Assert.That(result.ViewName,Is.EqualTo("AddItem"));
        }

        [Test]
        public void AddItem_TestLoadingCorrectModel_ReturnCorrectModel()
        {

            var additem = uut.AddItem() as ViewResult;
            Assert.AreEqual(null, additem);
        }

        [Test]
        public void Exit_NoNewItems_ReturnNull()
        {
            var obj = new AddItemController();

            RedirectToRouteResult result = obj.Exit() as RedirectToRouteResult;

            // Assert.Throws<NullReferenceException>(() => obj.Exit());
            Assert.AreEqual(result, null);
        }
    }
}
