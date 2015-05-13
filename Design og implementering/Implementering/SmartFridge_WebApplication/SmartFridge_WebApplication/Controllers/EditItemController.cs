using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using SmartFridge_WebApplication.Models;

namespace SmartFridge_WebApplication.Controllers
{
    public class EditItemController : Controller
    {
        static private GUIItem _oldItem = new GUIItem();

        private GUIItem _updatedItem = new GUIItem();

        //
        // GET: /EditItem/

        public ActionResult EditItem(GUIItem oldItem)
        {
            _oldItem = oldItem;
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
            Debug.WriteLine(collection["Type"]);
            Debug.WriteLine(collection["Amount"]);
            Debug.WriteLine(collection["Volume"]);
            Debug.WriteLine(collection["Unit"]);
            _updatedItem = new GUIItem(
                collection["Type"],
                Convert.ToUInt32(collection["Amount"]),
                Convert.ToUInt32(collection["Volume"]),
                collection["Unit"]
                );

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