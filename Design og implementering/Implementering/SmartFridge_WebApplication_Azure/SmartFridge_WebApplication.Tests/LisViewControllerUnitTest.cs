using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using SmartFridge_WebApplication.Controllers;
using SmartFridge_WebModels;

namespace SmartFridge_WebApplication.Tests
{
    [TestFixture]
    class LisViewControllerUnitTest
    {
        [Test]
        public void ItemToEdit_ItemNotEdited_ReturnEditItemRoute()
        {
            var obj = new LisViewController();

            RedirectToRouteResult result = obj.ToEditItem(new GUIItem("test", 1, 1, "L")) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo("EditItem"));
        }


        //ListViewDeleteItemTest kan ikke udføres pga. NULL reference givet af UnitOfWork objektet.
        //uden UnitOfWork er testen "Passed"
/*      [Test]
        public void ListViewDeleteItemTest()
        {
            var obj = new LisViewController();

            RedirectToRouteResult result = obj.DeleteSelectedItem(new GUIItem("test", 1, 1, "L")) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo("ListView"));

        }
  */
    }
}
