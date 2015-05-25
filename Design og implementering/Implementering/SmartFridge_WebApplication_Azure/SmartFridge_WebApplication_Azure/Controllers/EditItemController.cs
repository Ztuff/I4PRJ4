using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using SmartFridge_Cache;
using ListItem = SmartFridge_WebModels.ListItem;

namespace SmartFridge_WebApplication.Controllers
{
    public class EditItemController : Controller
    {
        private static GUIItem _oldGuiItem = new GUIItem();
        private static List<SelectListItem> _units;
        private static List<SelectListItem> _types;

        private GUIItem _updatedGUIItem = new GUIItem();

        //
        // GET: /EditItem/
        /// <summary>
        /// This function might be regarded as the views constructor, since it initializes everything the view needs.
        /// </summary>
        /// <param name="oldGuiItem">The item that if needed will be updated</param>
        /// <returns>Returns the EditItemView</returns>
        public ActionResult EditItem(GUIItem oldGuiItem)
        {

            _oldGuiItem = oldGuiItem;
            _types = new List<SelectListItem>();
            _types.Add(new SelectListItem { Text = "Varetype", Value = "Varetype", Selected = true });
            foreach (var item in Cache.DbItems)
            {
                _types.Add(new SelectListItem { Text = item.ItemName, Value = item.ItemName });
            }
            _units = new List<SelectListItem>(){new SelectListItem{Text = "l", Value = "l"},
                                                new SelectListItem{Text = "dl", Value = "dl"},
                                                new SelectListItem{Text = "ml", Value = "ml"},
                                                new SelectListItem{Text = "kg", Value = "kg"},
                                                new SelectListItem{Text = "g", Value = "g"},
                                                new SelectListItem{Text = "stk", Value = "stk"}};
            ViewData.Add("oldItem", _oldGuiItem);
            ViewBag.types = _types;
            ViewBag.units = _units;
            return View();
        }

        [HttpPost]
        public void Increment()
        {

        }

        [HttpPost]
        public void Decrement()
        {

        }
        /// <summary>
        /// Updates the selected item. If the item is updated in such a way that Type, ShelfLife, unit and size are similar to an already existing item,
        /// the amount of the existing item becomes the sum of its previous amount and the amount of the updated item. If this is the case the updated 
        /// item will be deleted.
        /// </summary>
        /// <param name="collection"> Collection of input from HTML form</param>
        /// <returns> Returns a redirect to the ListView</returns>
        [HttpPost]
        public ActionResult UpdateItem(FormCollection collection) //Mangler tjek på om den pågældene list item eksistere i forvejen
        {
            Item oldDbItem = new Item();
            //Loader de nye værdier og gemmer i et GUIItem
            string date = collection["date"];
            _updatedGUIItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["units"]
                );
            if (date.Length == 0)
            {
                _updatedGUIItem.ShelfLife = DateTime.MaxValue;
            }
            else
            {
                _updatedGUIItem.ShelfLife = Convert.ToDateTime(date);
            }
            //Finder det Item der svarer til typen på det GUIItem der skal ændres
            foreach (var item in Cache.DbItems)
            {
                if (item.ItemName == _oldGuiItem.Type)
                {
                    oldDbItem = item;
                    break;
                }
            }
            var uow = Cache.DalFacade.GetUnitOfWork();
            foreach (var listItem in Cache.CurrentListItems) //Leder alle ListItems på denne liste igennem for at finde det ListItem som bliver redigeret
            {
                if (listItem.Item.ItemId == oldDbItem.ItemId)//Hvis ListItem har samme typeID som det gamle dbItem
                {
                    if ((listItem.Item.ItemName == _updatedGUIItem.Type)) // Hvis ListItem også er af samme type som det nye redigerede GUIItem, ændrer vi værdierne
                    {
                        listItem.Amount = Convert.ToInt32(_updatedGUIItem.Amount);
                        listItem.Volume = Convert.ToInt32(_updatedGUIItem.Size);
                        listItem.Unit = _updatedGUIItem.Unit;
                        listItem.ShelfLife = _updatedGUIItem.ShelfLife;
                        uow.ListItemRepo.Update(listItem);
                        uow.SaveChanges();
                        Cache.DalFacade.DisposeUnitOfWork();
                        return RedirectToAction("ListView", "LisView");
                    }
                    else //Hvis typen er blevet ændret.
                    {
                        foreach (var item in Cache.DbItems)//leder alle Items igennem for at se om GUIItem er en ny type, eller om der allerede eksisterer en ItemType                            
                        {
                            if (item.ItemName == _updatedGUIItem.Type)//Hvis item er af samme type som det nye redigerede GUIItem ser vi om der eksisterer et ListItem, med samme type som det nye redigerede GUIITem                        
                            {
                                foreach (var originalListItem in Cache.CurrentListItems)//Leder alle ListItems igennem igen, for at se om et listItem med den nye type findes
                                {
                                    if (originalListItem.ItemId == item.ItemId &&   //Tjek på om der allerede eksistere en tilsavrende vare
                                        originalListItem.Unit == _updatedGUIItem.Unit &&    //gør der det opdateres den eksisterende vares antal
                                        originalListItem.Volume == _updatedGUIItem.Size &&  //med antallet fra den opdaterede vare og den 
                                        originalListItem.ShelfLife == _updatedGUIItem.ShelfLife)    //opdaterede vare slettes
                                    {
                                        originalListItem.Amount = originalListItem.Amount + (int)_updatedGUIItem.Amount;
                                        uow.ListItemRepo.Update(originalListItem);

                                        uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == listItem.ItemId &&
                                                                          l.ListId == listItem.ListId &&
                                                                          l.Unit == listItem.Unit &&
                                                                          l.Amount == listItem.Amount &&
                                                                          l.ShelfLife == listItem.ShelfLife &&
                                                                          l.Volume == listItem.Volume));
                                        uow.SaveChanges();
                                        Cache.DalFacade.DisposeUnitOfWork();

                                        return RedirectToAction("ListView", "LisView");
                                    }

                                }

                                Cache.DalFacade.DisposeUnitOfWork();
                                DeleteAndInsert(listItem,item);
                                return RedirectToAction("ListView", "LisView");
                            }
                        }

                        Cache.DalFacade.DisposeUnitOfWork();
                        DeleteAndInsert(listItem);
                        return RedirectToAction("ListView", "LisView");
                    }
                    return RedirectToAction("ListView", "LisView");
                }
                #region OldVersion
                //           if (listItem.Item.ItemName != _updatedItem.Type)
                //           {
                //               foreach (var item in Cache.DbItems)
                //               {
                //                   if (item.ItemName == _updatedItem.Type)
                //                   {
                //                       itemWithError = null;
                //                       foreach (var duplicateListItem in Cache.CurrentListItems)
                //                       {
                //                           if (duplicateListItem.ItemId == item.ItemId && //Tjek på om der allerede eksistere en tilsavrende vare
                //                               duplicateListItem.ListId == listItem.ListId && //gør der det opdateres den eksisterende vares antal
                //                               duplicateListItem.Unit == listItem.Unit &&               //med antallet fra den opdaterede vare og den 
                //                               duplicateListItem.Volume == listItem.Volume &&           //opdaterede vare slettes
                //                               duplicateListItem.ShelfLife == listItem.ShelfLife)

                //                           {
                //                               duplicateListItem.Amount = duplicateListItem.Amount + listItem.Amount;
                ////                               uow.ListItemRepo.Update(duplicateListItem);

                //                               uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == listItem.ItemId &&
                //                                                                 l.ListId == listItem.ListId &&
                //                                                                 l.Unit == listItem.Unit &&
                //                                                                 l.Amount == listItem.Amount &&
                //                                                                 l.ShelfLife == listItem.ShelfLife &&
                //                                                                 l.Volume == listItem.Volume));
                //                               uow.SaveChanges();
                //                               Cache.DalFacade.DisposeUnitOfWork();

                //                               return RedirectToAction("ListView", "LisView");
                //                           }
                //                       }
                //                       listItem.Item = item;
                //                   }
                //               }
                //               if (itemWithError != null)
                //               {
                //                   itemWithError.ItemName = _updatedItem.Type;
                //                   uow.ItemRepo.Update(itemWithError);
                //               }
                //         }
                //           listItem.Amount = Convert.ToInt32(_updatedItem.Amount);
                //           listItem.Volume = Convert.ToInt32(_updatedItem.Size);
                //           listItem.Unit = _updatedItem.Unit;
                //           uow.ListItemRepo.Update(listItem);
                //           break;
                //}}
                //uow.SaveChanges();
                //Cache.DalFacade.DisposeUnitOfWork();
                #endregion
            }
            //Hvis der ikke eksisterer et ListItem. Vi burde aldrig komme herned
            return RedirectToAction("ListView", "LisView");
        }

        /// <summary>
        /// Finds the selected items unit in the _units list and sets it as the selected.
        /// This function makes certain that it is the unit of the selected item that is
        /// displayed in the unit dropdown menu in the view.
        /// </summary>
        /// <param name="guiItem">The item that needs editing</param>
        public static void selectedUnit(GUIItem guiItem)
        {
            foreach (var unit in _units)
            {
                if (unit.Text == guiItem.Unit)
                {
                    unit.Selected = true;
                    break;
                }
            }

        }

        /// <summary>
        /// This functions deletes an old ListItem and inserts a new one, with an Item that did not exist in the database.
        /// </summary>
        /// <param name="listItemToDelete">The old item that needs to be deleted</param>
        private void DeleteAndInsert(ListItem listItemToDelete)
        {
            ListItem newListItem = new ListItem((int)_updatedGUIItem.Amount, 
                                                (int)_updatedGUIItem.Size, 
                                                _updatedGUIItem.Unit, 
                                                _updatedGUIItem.ShelfLife)
                                                {Item = new Item(_updatedGUIItem.Type), ListId = Cache.CurrentList.ListId};
            var uow = Cache.DalFacade.GetUnitOfWork();
            uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == listItemToDelete.ItemId &&
                                                                          l.ListId == listItemToDelete.ListId &&
                                                                          l.Unit == listItemToDelete.Unit &&
                                                                          l.Amount == listItemToDelete.Amount &&
                                                                          l.ShelfLife == listItemToDelete.ShelfLife &&
                                                                          l.Volume == listItemToDelete.Volume));
            uow.ListItemRepo.Add(newListItem);
            uow.SaveChanges();
            Cache.DalFacade.DisposeUnitOfWork();
        }

        /// <summary>
        /// This functions deletes an old ListItem and inserts a new one, with an Item that already was in the database.
        /// </summary>
        /// <param name="listItemToDelete">The old item that needs to be deleted</param>
        /// <param name="existingItem">The already existing item in the database</param>
        private void DeleteAndInsert(ListItem listItemToDelete, Item existingItem)
        {
            ListItem newListItem = new ListItem((int)_updatedGUIItem.Amount, 
                                                (int)_updatedGUIItem.Size, 
                                                _updatedGUIItem.Unit, 
                                                _updatedGUIItem.ShelfLife)
                                                {Item = new Item(existingItem.ItemName), ListId = Cache.CurrentList.ListId};

            var uow = Cache.DalFacade.GetUnitOfWork();
            uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == listItemToDelete.ItemId &&
                                                                          l.ListId == listItemToDelete.ListId &&
                                                                          l.Unit == listItemToDelete.Unit &&
                                                                          l.Amount == listItemToDelete.Amount &&
                                                                          l.ShelfLife == listItemToDelete.ShelfLife &&
                                                                          l.Volume == listItemToDelete.Volume));
            uow.ListItemRepo.Add(newListItem);
            uow.SaveChanges();
            Cache.DalFacade.DisposeUnitOfWork();
        }

    }
}