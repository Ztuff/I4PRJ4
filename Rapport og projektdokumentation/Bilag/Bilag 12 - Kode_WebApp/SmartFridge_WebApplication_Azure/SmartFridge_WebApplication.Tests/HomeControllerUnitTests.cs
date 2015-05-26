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
using SmartFridge_WebDAL.Context;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    class HomeControllerUnitTests
    {
        private HomeController uow;
        [SetUp]
        public void Setup()
        {
            uow = new HomeController();

            var dalfacade = Substitute.For<ISmartFridgeDALFacade>();

            Cache.DalFacade = dalfacade;
        }

        [Test]
        public void TestIndex()
        {


            var actResult = uow.Index() as ViewResult;

            Assert.That(actResult.ViewName, Is.EqualTo(""));
        }

        [Test]
        public void TestSetCurrentListReturnsTheRigtView()
        {
            string currentlist = "TestList";
            var actResult = uow.SetCurrentList(currentlist) as RedirectToRouteResult;

            Assert.That(actResult.RouteValues["action"], Is.EqualTo("ListView"));
        }

    }
}
