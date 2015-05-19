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
using SmartFridge_WebDAL;
using SmartFridge_WebModels;
using ListItem = SmartFridge_WebModels.ListItem;

namespace SmartFridge_WebApplication.Controllers
{
    public class EditItemController : Controller
    {
        private static GUIItem _oldItem = new GUIItem();
        private static List _currentList;
        private static ISmartFridgeDALFacade _dal;
        private static List<SelectListItem> _units;
        private static List<SelectListItem> _types;
        private static List<Item> _dbItems;
        private static List<ListItem> _dbListItems;
        private static List<List> _dbLists; 

        private GUIItem _updatedItem = new GUIItem();

        //
        // GET: /EditItem/

        public ActionResult EditItem(GUIItem oldItem)
        {

            _oldItem = oldItem;
            /*_currentList = currentList;
            _dal = dal;*/
            _dal = new SmartFridgeDALFacade();

            _oldItem = new GUIItem("test", 1, 1, "dl");//oldItem;
            _dal = new SmartFridgeDALFacade("SmartFridgeDb");

            _types = new List<SelectListItem>();
            var uow = _dal.GetUnitOfWork();
            _dbItems = uow.ItemRepo.GetAll().ToList();
            _dbListItems = uow.ListItemRepo.GetAll().ToList();
            _dbLists = uow.ListRepo.GetAll().ToList();
            _dal.DisposeUnitOfWork();
            _types.Add(new SelectListItem { Text = "Varetype", Value = "Varetype", Selected = true});
            foreach (var item in _dbItems)
            {
                int value = 0;
                _types.Add(new SelectListItem { Text = item.ItemName, Value = value.ToString() });
                value++;
            }

            foreach (var list in _dbLists)
            {
                if (list.ListName == TempData.Peek("CurrentListToEdit"))
                {
                    _currentList = list;
                }
            }
            _units = new List<SelectListItem>(){new SelectListItem{Text = "l", Value = "l"},
                                                new SelectListItem{Text = "dl", Value = "dl"},
                                                new SelectListItem{Text = "ml", Value = "ml"},
                                                new SelectListItem{Text = "kg", Value = "kg"},
                                                new SelectListItem{Text = "g", Value = "g"}};
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
        /// 
        /// </summary>
        /// <param name="collection"> Collection of input from HTML form</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateItem(FormCollection collection) //Mangler tjek på om den pågældene list item eksistere i forvejen
        {
            Item itemWithError = new Item();
            string date = collection["Shelflife"];
            _updatedItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["units"]
                );
            if(date.Length == 0)
            {             
                _updatedItem.ShelfLife = default(DateTime) /*Convert.ToDateTime(collection["Shelflife"])*/;
            }
            else
            {
                _updatedItem.ShelfLife = Convert.ToDateTime(collection["Shelflife"]);
            }
            foreach (var item in _dbItems)
            {
                if (item.ItemName == _oldItem.Type)
                {
                    itemWithError = item;
                }
            }
            var uow = _dal.GetUnitOfWork();
            foreach (var listItem in _dbListItems)
            {
                if (listItem.Item.ItemId == _oldItem.Id && listItem.List.ListId == _currentList.ListId)
                {
                    listItem.Amount = Convert.ToInt32(_updatedItem.Amount);
                    listItem.Volume = Convert.ToInt32(_updatedItem.Size);
                    listItem.Unit = _updatedItem.Unit;
                    if (listItem.Item.ItemName != _updatedItem.Type)
                    {
                        foreach (var item in _dbItems)
                        {
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
            _dal.DisposeUnitOfWork();

            return RedirectToAction("ListView", "LisView");
        }

    }
}