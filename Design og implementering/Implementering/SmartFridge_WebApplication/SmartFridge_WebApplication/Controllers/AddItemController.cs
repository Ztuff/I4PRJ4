using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartFridge_WebApplication.Models;

namespace SmartFridge_WebApplication.Controllers
{
    public class AddItemController : Controller
    {
        //
        // GET: /AddItem/

        private List<GUIItem> newGuiItems = new List<GUIItem>();

        public ActionResult AddItem()
        {
            IEnumerable<GUIItem> model = newGuiItems;
            return View(model);
        }

        public void AddGuiItem(string type, uint amount, uint size, string unit)
        {
         
            newGuiItems.Add(new GUIItem(type,amount,size,unit));
        }

        public List<GUIItem> AddAndExit(string type, uint amount, uint size, string unit)
        {
            newGuiItems.Add(new GUIItem(type, amount, size, unit));
            
            return newGuiItems;
        }
    }
}
