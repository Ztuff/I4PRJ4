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
using List = SmartFridge_WebModels.List;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    class LisViewControllerUnitTest
    {
        private LisViewController uut;
        [SetUp]
        public void Setup()
        {
            uut = new LisViewController();
            var dalfacade = Substitute.For<ISmartFridgeDALFacade>();
            Cache.DalFacade = dalfacade;
            List currentList = new SmartFridge_WebModels.List("TestList"){ListId = 0};
            Cache.CurrentList = currentList;
            Item item = new Item("TestType") {ItemId = 1};
            ListItem listitem = new ListItem(1, 1, "l", default(DateTime), null, item){ItemId = item.ItemId};
            dalfacade.GetUnitOfWork().ListRepo.GetAll().Returns(new List<List>() {currentList});
            dalfacade.GetUnitOfWork().ItemRepo.GetAll().Returns(new List<Item> {item});
            dalfacade.GetUnitOfWork().ListItemRepo.GetAll().Returns(new List<ListItem>() {listitem});
        }

        [Test]
        public void listView()
        {
            var actResult = uut.ListView() as ViewResult;

            Assert.That(actResult.ViewName, Is.EqualTo(""));
            
        }

        [Test]
        public void ItemToEdit_ItemNotEdited_ReturnEditItemRoute()
        {
            uut.ListView();

            RedirectToRouteResult result = uut.ToEditItem(new GUIItem("TestType", 1, 1, "l")) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo("EditItem"));
        }

        [Test]
        public void ItemToEdit_ItemDoesNotExist_ReturnEditItemRoute()
        {
            uut.ListView();

            RedirectToRouteResult result = uut.ToEditItem(new GUIItem("WrongTestType", 1, 1, "l")) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo("EditItem"));
        }



      [Test]
        public void ListViewDeleteItemTest()
        {
            uut.ListView();

            RedirectToRouteResult result = uut.DeleteSelectedItem(new GUIItem("TestType", 1, 1, "l")) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo("ListView"));

        }
  
    }
}
