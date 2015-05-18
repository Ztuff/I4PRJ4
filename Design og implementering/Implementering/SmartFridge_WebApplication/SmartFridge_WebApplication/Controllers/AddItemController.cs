using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SmartFridge_WebDAL;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebModels;

namespace SmartFridge_WebApplication.Controllers
{
    public class AddItemController : Controller
    {
        //
        // GET: /AddItem/
        static public string currentListName;
        static private int currentListID;
        static private List CurrentListEntity;
        static private List<GUIItem> newGuiItems = new List<GUIItem>();
        static private List<SelectListItem> ListGuiItemTypes = new List<SelectListItem>();
        static private IEnumerable<GUIItem> model;
        static private List<Item> ListItemTypes = new List<Item>();

        private ISmartFridgeDALFacade dalFacade = new SmartFridgeDALFacade("SmartFridgeDb");


        public ActionResult AddItem()
        {
            string testing = TempData.Peek("CurrentListToEdit").ToString();
            //currentList = TempData["CurrentListToEdit"].ToString();
            var uow = dalFacade.GetUnitOfWork();

            if (TempData.Peek("CurrentListToEdit") != null)
            {
                currentListName = TempData.Peek("CurrentListToEdit").ToString(); //Skal slettes når der er forbindelse til db
                

                List actualList = uow.ListRepo.Find(l => l.ListName == TempData.Peek("CurrentListToEdit").ToString());
                if (actualList != null)
                {
                    currentListID = actualList.ListId;
                    currentListName = actualList.ListName;
                    CurrentListEntity = actualList;
                }
            }
            
            //Test
            //ListItemTypes.Add(new Item("Is"));
            //ListItemTypes.Add(new Item("Vingummi"));
            //ListItemTypes.Add(new Item("Chokolade"));
            //ListItemTypes.Add(new Item("Kage"));
            //ListItemTypes = uow.ItemRepo.GetAll(); //Apparently not legal to do

            foreach (var guiItemTypes in uow.ItemRepo.GetAll()) 
            {
                ListGuiItemTypes.Add(new SelectListItem { Text = guiItemTypes.ItemName });
            }
            model = newGuiItems;
            ViewBag.ListNewGuiItems = ListGuiItemTypes;

            uow.SaveChanges();
            dalFacade.DisposeUnitOfWork();
            return View(model);
        }

        [HttpPost]
        public ActionResult addNewItem(string Varetype, string Antal, string Volume, string Enhed, string Holdbarhedsdato, string ItemImgClicked)
        {
            
            DateTime dblistItemDateTime = new DateTime();
            if (Holdbarhedsdato.Length == 0)
            {
                dblistItemDateTime = default(DateTime);
            }
            else
            {
                dblistItemDateTime = Convert.ToDateTime(Holdbarhedsdato);
            }


            GUIItem guiItemToAdd = new GUIItem();
            guiItemToAdd.ShelfLife = dblistItemDateTime; //AMOUNT READ FROM FIELD
            guiItemToAdd.Amount = Convert.ToUInt32(Antal); //Antal READ FROM FIELD
            guiItemToAdd.Size = Convert.ToUInt32(Volume); //Volume READ FROM FIELD
            guiItemToAdd.Type = Varetype; //Varetype READ FROM FIELD
            guiItemToAdd.Unit = Enhed; //unit READ FROM FIELD


            foreach (var newGuiItem in newGuiItems)
            {
                if (newGuiItem.Type.Equals(guiItemToAdd.Type) &&
                    newGuiItem.Size.Equals(guiItemToAdd.Size) &&
                    newGuiItem.Unit.Equals(guiItemToAdd.Unit) &&
                    newGuiItem.ShelfLife.Equals(guiItemToAdd.ShelfLife))
                {
                    newGuiItem.Amount += guiItemToAdd.Amount;                    
                    return null;//Viewet skal dog opdateres først
                }
            }
            newGuiItems.Add(guiItemToAdd);

            if (ItemImgClicked == "Exit")
            {

                return Exit(); 
            }

            //test
            //newGuiItems.Add(new GUIItem("Tester", 1, 6, "l"));

             model = newGuiItems;
             ViewBag.ListNewGuiItems = ListGuiItemTypes;
             return PartialView("~/Views/AddItem/AddItem.cshtml",model);

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


        //[HttpPost]
        //public ActionResult addItemAndExit(string Varetype, string Antal, string Volume, string Enhed, string Holdbarhedsdato)
        //{
        //    addNewItem(Varetype, Antal, Volume, Enhed, Holdbarhedsdato,"Add");
        //    Exit();
        //    return null;

        //    #region FromWPF

        //    /*AddNewItem(CreateNewItem());
        //    Exit();*/

        //    #endregion
        //}

        [HttpPost]
        public ActionResult Exit()
        {
            if (newGuiItems.Count == 0) {return null;} //Hvis der ikke er tilføjet nogle items, returnerer vi

            //Kobel de tilføjede items fra listen til databasen
            var uow = dalFacade.GetUnitOfWork();

            foreach (var newGuiItem in newGuiItems)
            {
                //Searching for item in DB
                Item dbItem = uow.ItemRepo.Find(l => l.ItemName == newGuiItem.Type);
                if (dbItem == null)
                {
                    dbItem = new Item(newGuiItem.Type);
                    uow.ItemRepo.Add(dbItem);
                }

                //Searching for ListItem in DB
                ListItem dbListItem = uow.ListItemRepo.Find(l => l.List.ListName == currentListName &&
                                                                 l.Item.ItemName == newGuiItem.Type &&
                                                                 l.Unit == newGuiItem.Unit &&
                                                                 l.Volume == newGuiItem.Size &&
                                                                 l.ShelfLife == newGuiItem.ShelfLife);
                if (dbListItem != null)
                {
                    dbListItem.Amount += (int)newGuiItem.Amount;
                }
                else if (dbListItem == null)
                {
                    dbListItem = new ListItem();
                    dbListItem.ShelfLife = newGuiItem.ShelfLife;
                    dbListItem.Amount = (int)newGuiItem.Amount;
                    dbListItem.Volume = (int)newGuiItem.Size;
                    dbListItem.Unit = newGuiItem.Unit;
                    dbListItem.ListId = currentListID;
                    dbListItem.List = CurrentListEntity;                    
                    dbListItem.ItemId = dbItem.ItemId;
                    dbListItem.Item = dbItem;

                    uow.ListItemRepo.Add(dbListItem); 
                }                                                           
                             
            }

            //uow.SaveChanges();
            dalFacade.DisposeUnitOfWork();

          //  return View("~/Views/LisView/ListView.cshtml", new { ListToEdit = currentListName });
            return RedirectToAction("ListView", "LisView", new { ListToEdit = currentListName });


            #region FromWPF

            /*_ctrlTemp._bll.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));*/

            #endregion
        }
    }
}
