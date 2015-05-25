using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using SmartFridge_Cache;

namespace SmartFridge_WebApplication.Controllers
{
    public class LisViewController : Controller
    {
        private static IEnumerable<GUIItem> model = new List<GUIItem>(); 

        /// <summary>
        /// Henter alle listItems fra Cache ind, og opdaterer modellen, som bliver vist i viewet
        /// </summary>
        /// <param name="ListToEdit"></param>
        /// <returns></returns>
        public ActionResult ListView()
        {
            Cache.CurrentList = Cache.CurrentList;
            List<GUIItem> tempData = new List<GUIItem>();
            //foreach (var item in _dbItems)
            //{
            //    tempData.Add(new GUIItem(item.ItemName,0,(uint)item.StdVolume,item.StdUnit){ItemId = item.ItemId});
            //}

            if(Cache.CurrentListItems.Any())
            { 
                foreach (var listItem in Cache.CurrentListItems)
                {
                    foreach (var item in Cache.DbItems)
                    {
                        if(item.ItemId == listItem.ItemId)
                        { 
                            GUIItem temp = new GUIItem(item.ItemName, (uint)listItem.Amount, (uint)listItem.Volume, listItem.Unit){ItemId = item.ItemId, ShelfLife = listItem.ShelfLife};
                            tempData.Add(temp);
                        }
                        
                    }
                }
            }

            model = tempData; 
            //model = new List<GUIItem>() { new GUIItem("KONTENT'SSSSS", 1, 1, "Reference"), new GUIItem("TreadsSS!", 2, 3, "Reference") { ShelfLife = new DateTime(2017, 6, 2) } }; //Til test


            return View(model);
        }
        /// <summary>
        /// Funktionen sammenligner parameteren GUIItem med de items der ligger i modelen.
        /// </summary>
        /// <param name="itemToEdit"></param>
        /// <returns>EditItem viewet med det GUIitem der skal redigeres</returns>
        public ActionResult ToEditItem(GUIItem itemToEdit)
        {
            foreach (var item in model)
            {
                if (item.Type == itemToEdit.Type)/* && item.Amount == itemToEdit.Amount && item.Size == itemToEdit.Size && item.Unit == itemToEdit.Unit*/
                {
                     return RedirectToAction("EditItem","EditItem", item);
                }
            }
           // return RedirectToAction("ListView","LisView");
            return RedirectToAction("EditItem", "EditItem", itemToEdit);
        }
        /// <summary>
        /// Funktionen sammenligner parameteren GUIItem med de items der ligger i modelen. Findes det item der ledes efter
        /// bliver det fjernet fra listen med GUIItems. Da listen ligger i en IEnumerable konverteres den til en Liste
        /// hvorefter itemToDelete bliver fjernet fra listen, og den nye liste bliver lagt i model igen.
        /// </summary>
        /// <param name="itemToDelete"></param>
        /// <returns>ListView hvor listen med GUIItems nu er opdateret</returns>
        public ActionResult DeleteSelectedItem(GUIItem itemToDelete)
        {
            var uow = Cache.DalFacade.GetUnitOfWork();
            foreach (var item in model)
            {
                if (item.Type == itemToDelete.Type && item.Amount == itemToDelete.Amount && item.Size == itemToDelete.Size && item.Unit == itemToDelete.Unit)
                {
                    List<GUIItem> tempList = model.ToList();
                    tempList.Remove(itemToDelete);
                    model = tempList;
                    foreach (var dblistitem in Cache.CurrentListItems)
                    {
                        if (dblistitem.ItemId == item.ItemId)
                        {
                            //uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == 9));
                            uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == dblistitem.ItemId &&
                                                                          l.ListId == dblistitem.ListId &&
                                                                          l.Unit == dblistitem.Unit &&
                                                                          l.Amount == dblistitem.Amount &&
                                                                          l.ShelfLife == dblistitem.ShelfLife &&
                                                                          l.Volume == dblistitem.Volume));
                        } 
                    }
                }
            }
            uow.SaveChanges();
            Cache.DalFacade.DisposeUnitOfWork();
            return RedirectToAction("ListView", "LisView");
        }
    }
}