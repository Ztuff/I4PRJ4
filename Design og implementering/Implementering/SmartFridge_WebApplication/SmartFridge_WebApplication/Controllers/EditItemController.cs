using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        static private GUIItem _oldItem = new GUIItem();
        static private GUIItemList _currentList;
        static private ISmartFridgeDALFacade _dal;

        private GUIItem _updatedItem = new GUIItem();

        //
        // GET: /EditItem/

        public ActionResult EditItem(GUIItem oldItem/*, GUIItemList currentList, 
                                     ISmartFridgeDALFacade dal*/)
        {
            _oldItem = new GUIItem("test", 1, 1, "l");//oldItem;
            /*_currentList = currentList;
            _dal = dal;*/
            _currentList = new GUIItemList(0, "testlist");
            _dal = new SmartFridgeDALFacade();
            ViewData.Add("oldItem", _oldItem);
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

            _updatedItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["Unit"]
                );

            var uow = _dal.GetUnitOfWork();
            List<SmartFridge_WebApplication.Models.ListItem> dbListItems = uow.ListItemRepo.GetAll().ToList();
            List<Item> dbItems = uow.ItemRepo.GetAll().ToList();
            foreach (var listItem in dbListItems)
            {
                if (listItem.Item.ItemId == _oldItem.ID && listItem.List.ListId == _currentList.ID)
                {
                    listItem.Amount = Convert.ToInt32(_updatedItem.Amount);
                    listItem.Volume = Convert.ToInt32(_updatedItem.Size);
                    listItem.Unit = _updatedItem.Unit;
                    if (listItem.Item.ItemName != _updatedItem.Type)
                    {
                        foreach (var item in dbItems)
                        {
                            if (item.ItemName == _updatedItem.Type)
                            {
                                listItem.Item = item;
                            }
                            else
                            {
                                var temp = new Item(_updatedItem.Type);
                                uow.ItemRepo.Add(temp);
                            }
                        }
                    }
                    uow.ListItemRepo.Update(listItem);
                    break;
                }
            }

            uow.SaveChanges();

            _dal.DisposeUnitOfWork();

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