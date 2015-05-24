using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using SmartFridge_Cache;
using ListItem = SmartFridge_WebModels.ListItem;

namespace SmartFridge_WebApplication.Controllers
{
    public class EditItemController : Controller
    {
        private static GUIItem _oldItem = new GUIItem();
        private static List<SelectListItem> _units;
        private static List<SelectListItem> _types;

        private GUIItem _updatedGUIItem = new GUIItem();

        //
        // GET: /EditItem/

        public ActionResult EditItem(GUIItem oldItem)
        {

            _oldItem = oldItem;
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
            ViewData.Add("oldItem", _oldItem);
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
            Item itemWithError = new Item();
            string date = collection["Shelflife"];
            _updatedGUIItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["units"]
                );
            if (date.Length == 0)
            {
                _updatedGUIItem.ShelfLife = default(DateTime);
            }
            else
            {
                _updatedGUIItem.ShelfLife = Convert.ToDateTime(collection["Shelflife"]);
            }
            foreach (var item in Cache.DbItems)
            {
                if (item.ItemName == _oldItem.Type)
                {
                    itemWithError = item;
                    break;
                }
            }
            var uow = Cache.DalFacade.GetUnitOfWork();
            foreach (var listItem in Cache.CurrentListItems)
            {
                if (listItem.Item.ItemId == _oldItem.ItemId)
                {
                    if ((listItem.Item.ItemName == _updatedGUIItem.Type))
                    {
                        listItem.Amount = Convert.ToInt32(_updatedGUIItem.Amount);
                        listItem.Volume = Convert.ToInt32(_updatedGUIItem.Size);
                        listItem.Unit = _updatedGUIItem.Unit;
                        uow.ListItemRepo.Update(listItem);
                        uow.SaveChanges();
                        Cache.DalFacade.DisposeUnitOfWork();
                        return RedirectToAction("ListView", "LisView");
                    }
                    else
                    {
                        foreach (var item in Cache.DbItems)
                        {
                            if (item.ItemName == _updatedGUIItem.Type)
                            {
                                foreach (var originalListItem in Cache.CurrentListItems)
                                {
                                    if (originalListItem.ItemId == item.ItemId && //Tjek på om der allerede eksistere en tilsavrende vare
                                        originalListItem.Unit == _updatedGUIItem.Unit && //gør der det opdateres den eksisterende vares antal
                                        originalListItem.Volume == _updatedGUIItem.Size && //med antallet fra den opdaterede vare og den 
                                        originalListItem.ShelfLife == _updatedGUIItem.ShelfLife)//opdaterede vare slettes
                                    {
                                        originalListItem.Amount = originalListItem.Amount + (int)_updatedGUIItem.Amount;
                                        uow.ListItemRepo.Update(originalListItem);

                                        /*uow.ListItemRepo.Delete(uow.ListItemRepo.Find(l => l.ItemId == listItem.ItemId &&
                                                                          l.ListId == listItem.ListId &&
                                                                          l.Unit == listItem.Unit &&
                                                                          l.Amount == listItem.Amount &&
                                                                          l.ShelfLife == listItem.ShelfLife &&
                                                                          l.Volume == listItem.Volume));*/
                                        uow.SaveChanges();
                                        Cache.DalFacade.DisposeUnitOfWork();

                                        return RedirectToAction("ListView", "LisView");
                                    }

                                }
                            }
                            else
                            {
                                itemWithError.ItemName = _updatedGUIItem.Type;
                                uow.ItemRepo.Update(itemWithError);
                                listItem.Amount = Convert.ToInt32(_updatedGUIItem.Amount);
                                listItem.Volume = Convert.ToInt32(_updatedGUIItem.Size);
                                listItem.Unit = _updatedGUIItem.Unit;
                                uow.ListItemRepo.Update(listItem);
                                uow.SaveChanges();
                                Cache.DalFacade.DisposeUnitOfWork();
                                return RedirectToAction("ListView", "LisView");
                            }
                        }
                        return RedirectToAction("ListView", "LisView");
                    }
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
                }
            }
            //uow.SaveChanges();
            //Cache.DalFacade.DisposeUnitOfWork();

            return RedirectToAction("ListView", "LisView");
        }

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

    }
}