using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SmartFridge_WebDAL;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebModels;
using SmartFridge_Cache;

namespace SmartFridge_WebApplication.Controllers
{
    public class AddItemController : Controller
    {
        //
        // GET: /AddItem/
        static private List<GUIItem> newGuiItems = new List<GUIItem>();
        static private List<SelectListItem> ListGuiItemTypes = new List<SelectListItem>();
        static private IEnumerable<GUIItem> model;
        static private List<Item> ListItemTypes = new List<Item>();

        /// <summary>
        /// Loader alle Items fra databsen, for at hente alle typerne ind i varetype drop down.
        /// Og sætter viewet
        /// </summary>
        /// <returns>addItem view</returns>
        public ActionResult AddItem()
        {
            var uow = Cache.DalFacade.GetUnitOfWork();
            ListGuiItemTypes.Add(new SelectListItem { Text = "Varetype", Value = "Varetype", Selected = true });
            foreach (var guiItemTypes in uow.ItemRepo.GetAll()) 
            {
                ListGuiItemTypes.Add(new SelectListItem { Text = guiItemTypes.ItemName });
            }
            model = newGuiItems;
            ViewBag.ListNewGuiItems = ListGuiItemTypes;

            uow.SaveChanges();
            Cache.DalFacade.DisposeUnitOfWork();
            return View(model);
        }
        /// <summary>
        /// Henter alle dataene indtastet i inputfelterterne. Hvis et GUIItem med samme værdier allerede er tilføjet
        /// ligges den nye amount blot til det allerede eksisterende GUIItem. Ellers laves et nyt og tilføjes til
        /// listen med nyligt tilføjede items.
        /// </summary>
        /// <param name="Varetype"></param>
        /// <param name="Antal"></param>
        /// <param name="Volume"></param>
        /// <param name="Enhed"></param>
        /// <param name="Holdbarhedsdato"></param>
        /// <param name="ItemImgClicked"></param>
        /// <returns>Hvis der er trykket på tilføj og afslut returneres exit funktionen, ellers returneres et partialview med den opdaterede GUIItem liste</returns>
        [HttpPost]
        public ActionResult addNewItem(string Varetype, string Antal, string Volume, string Enhed, string Holdbarhedsdato, string ItemImgClicked)
        {
            DateTime dblistItemDateTime = new DateTime();
            if (Holdbarhedsdato.Length == 0)
            {
                dblistItemDateTime = DateTime.MaxValue;
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
        #region Outdated addItemAndExit
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
        #endregion

        /// <summary>
        /// Mapper GUIItems fra GUIItem listen med nyligt tilføjede vare, til Item, List og List Item i repository.
        /// Herefter gemmes ændringerne, og de tilføjes til databasen
        /// </summary>
        /// <returns>LisView</returns>
        [HttpPost]
        public ActionResult Exit()
        {
            if (newGuiItems.Count == 0) {return null;} //Hvis der ikke er tilføjet nogle items, returnerer vi

            //Kobel de tilføjede items fra listen til databasen
            var uow = Cache.DalFacade.GetUnitOfWork();

            foreach (var newGuiItem in newGuiItems)
            {
                //Searching for item in DB
                Item dbItem = uow.ItemRepo.Find(l => l.ItemName == newGuiItem.Type);
                if (dbItem == null)
                {
                    dbItem = new Item(newGuiItem.Type){StdUnit = newGuiItem.Unit, StdVolume = (int)newGuiItem.Size};
                    uow.ItemRepo.Add(dbItem);
                }

                //Searching for ListItem in DB
                ListItem dbListItem = uow.ListItemRepo.Find(l => l.List.ListName == Cache.CurrentList.ListName &&
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
                    dbListItem.ListId = Cache.CurrentList.ListId;
                    dbListItem.ItemId = dbItem.ItemId;
                    dbListItem.Item = dbItem;
                    
                    uow.ListItemRepo.Add(dbListItem); 
                }                                                           
                             
            }

            uow.SaveChanges();
            Cache.DalFacade.DisposeUnitOfWork();

          //  return View("~/Views/LisView/ListView.cshtml", new { ListToEdit = currentListName });
            newGuiItems = new List<GUIItem>();
            model = newGuiItems;
            return RedirectToAction("ListView", "LisView");


            #region FromWPF

            /*_ctrlTemp._bll.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));*/

            #endregion
        }
    }
}
