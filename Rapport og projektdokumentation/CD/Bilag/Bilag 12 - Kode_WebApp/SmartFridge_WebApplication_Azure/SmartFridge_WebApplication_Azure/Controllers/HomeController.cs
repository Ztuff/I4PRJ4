using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartFridge_Cache;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;

namespace SmartFridge_WebApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            Cache.DalFacade = new SmartFridgeDALFacade("SmartFridgeDb");

            return View();
        }

        public ActionResult SetCurrentList(string listToEdit)
        {
            List currentList = new List();

            var uow = Cache.DalFacade.GetUnitOfWork();

            currentList = uow.ListRepo.Find(l => l.ListName == listToEdit);

            Cache.DalFacade.DisposeUnitOfWork();

            Cache.CurrentList = currentList;

            return RedirectToAction("ListView","LisView");

           
        }
    }
}
