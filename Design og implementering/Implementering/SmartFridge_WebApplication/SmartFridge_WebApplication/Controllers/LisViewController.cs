using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using SmartFridge_WebModels;

namespace SmartFridge_WebApplication.Controllers
{
    public class LisViewController : Controller
    {
        public string currentList;
        private IEnumerable<GUIItem> model = new List<GUIItem>() { new GUIItem("KONTENT'SSSSS", 1, 1, "Reference"), new GUIItem("TreadsSS!", 2, 3, "Reference") { ShelfLife = new DateTime(2017, 6, 2) } };
        public ActionResult ListView(string ListToEdit)
        {
            currentList = ListToEdit;
            TempData["CurrentListToEdit"] = ListToEdit;
            return View(model);
        }

        public ActionResult ToEditItem(GUIItem itemToEdit)
        {
            foreach (var item in model)
            {
                if (item.Type == itemToEdit.Type && item.Amount == itemToEdit.Amount && item.Size == itemToEdit.Size && item.Unit == itemToEdit.Unit)
                {
                     return RedirectToAction("EditItem","EditItem", item);
                }
            }
            return RedirectToAction("ListView","LisView");
        }
    }
}
