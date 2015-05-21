using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using SmartFridge_WebApplication.Controllers;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    class AddItemControllerUnitTest
    {

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
