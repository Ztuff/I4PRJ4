using System;
using System.Collections.Generic;
using System.Data.Objects;
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
        private List<SelectListItem> ListGuiItemTypes = new List<SelectListItem>();
        private IEnumerable<GUIItem> model;
        private List<Item> ListItemTypes = new List<Item>();
        //private IEnumerable<GUIItem> model;
        public ActionResult AddItem()
        {
                   model = newGuiItems;
            
            //Test
            ListItemTypes.Add(new Item("test1",42,"kgb"));
            ListItemTypes.Add(new Item("test2",42,"kgb"));
            ListItemTypes.Add(new Item("test3",42,"kgb"));
            ListItemTypes.Add(new Item("test4",42,"kgb"));


            foreach  (var GuiItemTypes in ListItemTypes ) 
            {
                ListGuiItemTypes.Add(new SelectListItem { Text = GuiItemTypes.ItemName });
            }

            ViewBag.ListNewGuiItems = ListGuiItemTypes;
            return View(model);
        }
        #region interne funktioner_OLD
      /*  public void AddGuiItem(string type, uint amount, uint size, string unit)
        {
         
            newGuiItems.Add(new GUIItem(type,amount,size,unit));
        }

        public List<GUIItem> AddAndExit(string type, uint amount, uint size, string unit)
        {
            newGuiItems.Add(new GUIItem(type, amount, size, unit));
            
            return newGuiItems;
        }*/
        #endregion

        [HttpPost]
        public ActionResult addNewItem(string Varetype, string Antal, string Volume ,string unit)
        {
            GUIItem guiItemToAdd = new GUIItem();
            //guiItemToAdd.ShelfLife = AMOUNT READ FROM FIELD
            guiItemToAdd.Amount = Convert.ToUInt32(Antal); //Antal READ FROM FIELD
            guiItemToAdd.Size = Convert.ToUInt32(Volume); //Volume READ FROM FIELD
            guiItemToAdd.Type = Varetype; //Varetype READ FROM FIELD
            guiItemToAdd.Unit = unit; //unit READ FROM FIELD

            foreach (var i in newGuiItems)
            {
                if (i.Type.Equals(guiItemToAdd.Type) &&
                    i.Size.Equals(guiItemToAdd.Size) &&
                    i.Unit.Equals(guiItemToAdd.Unit) &&
                    i.ShelfLife.Equals(guiItemToAdd.ShelfLife))
                {
                    i.Amount += guiItemToAdd.Amount;
                    //ListBoxItems.Items.Refresh();
                    return null;//Viewet skal dog opdateres først
                }
            }
            newGuiItems.Add(guiItemToAdd);
            //test
            newGuiItems.Add(new GUIItem("Tester", 1, 6, "l"));
             //ListBoxItems.Items.Refresh();
             model = newGuiItems;
            ViewBag.ListNewGuiItems = ListGuiItemTypes;
             return RedirectToAction("AddItem"); //Viewet skal dog opdateres først

            #region FromWPF             

            /*foreach (var i in newItems)
            {
                if (i.Type.Equals(item.Type))
                {
                    i.Amount += item.Amount;
                    ListBoxItems.Items.Refresh();
                    return;
                }
            }
            newItems.Add(item);

            ListBoxItems.Items.Refresh();*/

            #endregion
        }


        [HttpPost]
        public ActionResult addItemAndExit()
        {
            AddItem();
            Exit();
            return null;

            #region FromWPF

            /*AddNewItem(CreateNewItem());
            Exit();*/

            #endregion


        }
        [HttpPost]
        public ActionResult Exit()
        {
            //Kobel de tilføjede items fra listen til databasen
            return View("~/Views/LisView/ListView.cshtml");

            #region FromWPF

            /*_ctrlTemp._bll.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));*/

            #endregion


        }

        /*private GUIItem CreateNewItem()
        {
            
            return _ctrlTemp._bll.CreateNewItem(TextBoxVareType.Text, Convert.ToUInt32(TextBoxAntal.Text),
                Convert.ToUInt32(TextBoxVolumen.Text), TextBoxVolumenEnhed.Text, TextBoxShelfLife.SelectedDate);
        }*/
        //public ActionResult SelectType()
        //{
        //                ViewBag.ListNewGuiItems = newGuiItems;
        //                return View();

        //}
 





    }
}
