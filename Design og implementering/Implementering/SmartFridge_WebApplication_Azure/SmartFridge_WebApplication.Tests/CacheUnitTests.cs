using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class CacheUnitTests
    {
        List currentList = new SmartFridge_WebModels.List("TestList") { ListId = 0 };
        Item item = new Item("TestType") { ItemId = 1 };
        ListItem listitem = new ListItem();

        [SetUp]
        public void Setup()
        {
            var dalfacade = Substitute.For<ISmartFridgeDALFacade>();
            Cache.DalFacade = dalfacade;
           // currentList = new SmartFridge_WebModels.List("TestList"){ListId = 0};
            //Item item = new Item("TestType") {ItemId = 1};
            listitem = new ListItem(1, 1, "l", default(DateTime), currentList, item) { ItemId = item.ItemId };
            dalfacade.GetUnitOfWork().ListRepo.GetAll().Returns(new List<List>() {currentList});
            dalfacade.GetUnitOfWork().ItemRepo.GetAll().Returns(new List<Item> {item});
            dalfacade.GetUnitOfWork().ListItemRepo.GetAll().Returns(new List<ListItem>() {listitem});
        }

        [Test]
        public void CurrentListSet_ContainsListItem()
        {
            Cache.CurrentList = currentList;
            Assert.That(Cache.CurrentListItems.Contains(listitem));
        }

        [Test]
        public void CurrentListSet_ContainsTestTypeItem()
        {
            Cache.CurrentList = currentList;
            Assert.That(Cache.DbItems.Contains(item));
        }

        [Test]
        public void CurrentListSet_CorrectList()
        {
                        Cache.CurrentList = currentList;
            Assert.AreEqual(currentList, Cache.CurrentList);
        }
    }
}
