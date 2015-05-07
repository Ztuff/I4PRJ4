using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartFridge_WebApplication.Models;
using System.Web.Helpers;

namespace SmartFridge_WebApplication.Controllers
{
    public class LisViewController : Controller
    {
        //
        // GET: /LisView/



        public ActionResult ListView()
        {
            IEnumerable<GUIItem> model = new List<GUIItem>(){new GUIItem("KONTENT'SSSSS",1,1,"Reference")};
            return View(model);
        }


    }
}
