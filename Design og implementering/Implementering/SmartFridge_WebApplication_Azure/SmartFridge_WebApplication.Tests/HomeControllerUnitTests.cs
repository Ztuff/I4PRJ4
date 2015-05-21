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
    class HomeControllerUnitTests
    {

        [Test]
        public void TestIndex()
        {
            var obj = new HomeController();

            var actResult = obj.Index() as ViewResult;

            Assert.That(actResult.ViewName, Is.EqualTo(""));
        }

    }
}
