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

namespace SmartFridge_WebApplication.Controllers
{
    public class LisViewController : Controller
    {
        public string currentList;
        private IEnumerable<GUIItem> model = new List<GUIItem>(); 
        private static ISmartFridgeDALFacade _dal;
        private static List<Item> _dbItems;
        private static List<ListItem> _dbListItems;
        private static List<List> _dbLists; 

        public ActionResult ListView(string ListToEdit)
        {
            currentList = ListToEdit;
            TempData["CurrentListToEdit"] = ListToEdit;
            List<GUIItem> tempData = new List<GUIItem>();
            _dal = new SmartFridgeDALFacade("SmartFridgeDb");
            var uow = _dal.GetUnitOfWork();
            _dbItems = uow.ItemRepo.GetAll().ToList();
            _dbListItems = uow.ListItemRepo.GetAll().ToList();
            _dbLists = uow.ListRepo.GetAll().ToList();
            _dal.DisposeUnitOfWork();

            foreach (var item in _dbItems)
            {
                tempData.Add(new GUIItem(item.ItemName,0,(uint)item.StdVolume,item.StdUnit));
            }
            foreach (var item in _dbListItems)
            {
                int i = 0;
                if (item.ItemId == tempData[i].Id)
                {
                    tempData[i].Amount = (uint)item.Amount;
                    tempData[i].Size = (uint)item.Volume;
                    //tempData[i].ShelfLife = item.ShelfLife; //Nullable DateTime vs DateTime
                    tempData[i].Unit = item.Unit;

                }
                i++;
            }
            model = tempData; 
            //model = new List<GUIItem>() { new GUIItem("KONTENT'SSSSS", 1, 1, "Reference"), new GUIItem("TreadsSS!", 2, 3, "Reference") { ShelfLife = new DateTime(2017, 6, 2) } }; //Til test
            return View(model);
        }

        public ActionResult ToEditItem(GUIItem itemToEdit)
        {
            foreach (var item in model)
            {
                if (item.Type == itemToEdit.Type/* && item.Amount == itemToEdit.Amount && item.Size == itemToEdit.Size && item.Unit == itemToEdit.Unit*/)
                {
                     return RedirectToAction("EditItem","EditItem", item);
                }
            }
           // return RedirectToAction("ListView","LisView");
            return RedirectToAction("EditItem", "EditItem", itemToEdit);
        }

        public ActionResult DeleteSelectedItem(GUIItem itemToDelete)
        {
            foreach (var item in model)
            {
                if (item.Type == itemToDelete.Type && item.Amount == itemToDelete.Amount && item.Size == itemToDelete.Size && item.Unit == itemToDelete.Unit)
                {
                    List<GUIItem> tempList = model.ToList();
                    tempList.Remove(itemToDelete);
                    model = tempList;
                }
            }
            return RedirectToAction("ListView", "LisView");
        }
    }
}
