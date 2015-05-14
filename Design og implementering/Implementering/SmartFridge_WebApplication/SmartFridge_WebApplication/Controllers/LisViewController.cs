﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartFridge_WebApplication.Models;
using System.Web.Helpers;

namespace SmartFridge_WebApplication.Controllers
{
    public class LisViewController : Controller
    {
        public string currentList;
        public ActionResult ListView(string ListToEdit)
        {
            currentList = ListToEdit;
            TempData["CurrentListToEdit"] = ListToEdit;
            IEnumerable<GUIItem> model = new List<GUIItem>(){new GUIItem("KONTENT'SSSSS",1,1,"Reference"), new GUIItem("TreadsSS!", 1, 1, "Reference"){ShelfLife = new DateTime(2017,6,2)}};
            return View(model);
        }
    }
}
