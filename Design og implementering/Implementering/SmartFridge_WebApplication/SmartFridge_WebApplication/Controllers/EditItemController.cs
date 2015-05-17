using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using SmartFridge_WebApplication.Models;
using SmartFridge_WebApplication.DAL;

namespace SmartFridge_WebApplication.Controllers
{
    public class EditItemController : Controller
    {
        private static GUIItem _oldItem = new GUIItem();
        private static Models.List _currentList;
        private static ISmartFridgeDALFacade _dal;
        private static List<SelectListItem> _units;
        private static List<SelectListItem> _types;
        private static List<Item> _dbItems;
        private static List<Models.ListItem> _dbListItems;
        private static List<Models.List> _dbLists; 

        private GUIItem _updatedItem = new GUIItem();

        //
        // GET: /EditItem/

        public ActionResult EditItem(GUIItem oldItem/*, GUIItemList currentList, 
                                     ISmartFridgeDALFacade dal*/)
        {
            _oldItem = new GUIItem("test", 1, 1, "l");//oldItem;
            /*_currentList = currentList;
            _dal = dal;*/
            _dal = new SmartFridgeDALFacade();
            _types = new List<SelectListItem>();
            //var uow = _dal.GetUnitOfWork();
            //_dbItems = uow.ItemRepo.GetAll().ToList();
            //_dbListItems = uow.ListItemRepo.GetAll().ToList();
            //_dbLists = uow.ListRepo.GetAll().ToList();
            //_dal.DisposeUnitOfWork();
            //foreach (var item in _dbItems)
            //{
            //    int value = 0;
            //    _types.Add(new SelectListItem{Text = item.ItemName, Value = value.ToString()});
            //    value++;
            //}

            //foreach (var list in _dbLists)
            //{
            //    if (list == TempData["CurrentListToEdit"])
            //    {
            //        _currentList = list;
            //    }
            //}

            _units = new List<SelectListItem>(){new SelectListItem{Text = "l", Value = "0"},
                                                new SelectListItem{Text = "dl", Value = "1"},
                                                new SelectListItem{Text = "ml", Value = "2"},
                                                new SelectListItem{Text = "kg", Value = "3"},
                                                new SelectListItem{Text = "g", Value = "4"}};
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

        [HttpPost]
        public ActionResult UpdateItem(FormCollection collection)
        {
            Item itemWithError = new Item();

            _updatedItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["Unit"]
                )/*{ShelfLife = Convert.ToDateTime(collection["Shelflife"])}*/;

            var uow = _dal.GetUnitOfWork();
            foreach (var listItem in _dbListItems)
            {
                if (listItem.Item.ItemId == _oldItem.ID && listItem.List.ListId == _currentList.ListId)
                {
                    listItem.Amount = Convert.ToInt32(_updatedItem.Amount);
                    listItem.Volume = Convert.ToInt32(_updatedItem.Size);
                    listItem.Unit = _updatedItem.Unit;
                    if (listItem.Item.ItemName != _updatedItem.Type)
                    {
                        foreach (var item in _dbItems)
                        {
                            

                            if (item.ItemName == _oldItem.Type)
                            {
                                itemWithError = item;
                            }
                            if (item.ItemName == _updatedItem.Type)
                            {
                                listItem.Item = item;
                                uow.ItemRepo.Delete(itemWithError);
                                itemWithError = null;
                            }
                        }
                        if (itemWithError != null)
                        {
                            itemWithError.ItemName = _updatedItem.Type;
                            uow.ItemRepo.Update(itemWithError);
                        }
                    }
                    uow.ListItemRepo.Update(listItem);
                    break;
                }
            }

            //uow.SaveChanges();

            //_dal.DisposeUnitOfWork();

            //Jeg forestiller mig at strukturen bliver for fjern gammel 
            //indsæt ny bliver omtrent den samme, og at skellettet fra
            //den lokale app, derfor kan bruges.

            //foreach (var dbListItem in _dblistItems)
            //{
            //    if (dbListItem.Item.ItemName == oldItem.Type &&
            //        dbListItem.Unit == oldItem.Unit &&
            //        (uint)dbListItem.Volume == oldItem.Size &&
            //        dbListItem.Amount == oldItem.Amount &&
            //        dbListItem.ListId == currentGuiItemList.ID)
            //    {
            //        if (oldItem.Type != newItem.Type) //Hvis der skal ændres i dens Item (og ikke kun i listItem)
            //        {
            //            foreach (var dbItem in _dbItems)
            //            {
            //                if (dbItem.ItemName == oldItem.Type)
            //                {
            //                    dbItem.ItemName = newItem.Type;
            //                    _itemRepository.Update(dbItem);
            //                }
            //            }
            //        }

            //        ListItem updatedListItem = new ListItem((int)newItem.Amount,
            //                                                (int)newItem.Size,
            //                                                newItem.Unit,
            //                                                dbListItem.List,
            //                                                dbListItem.Item);

            //        _listItemRepository.Delete(dbListItem);
            //        _listItemRepository.Insert(updatedListItem);
            //        uow.SaveChanges();
            //        break;
            //    }
            //} 

            return RedirectToAction("ListView", "LisView");
        }

    }
}