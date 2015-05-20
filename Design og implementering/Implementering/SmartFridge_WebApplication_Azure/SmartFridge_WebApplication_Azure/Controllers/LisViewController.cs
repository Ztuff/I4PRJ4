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

        /// <summary>
        /// Funktionen henter den Item listen fra DAL laget og ligger items ind 
        /// i 'model' som skal vises i Viewet.
        /// </summary>
        /// <param name="ListToEdit"></param>
        /// <returns></returns>
        public ActionResult ListView(string ListToEdit)
        {
            currentList = ListToEdit;
            TempData["CurrentListToEdit"] = ListToEdit;
			TempData.Keep();
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
        /// <summary>
        /// Funktionen sammenligner parameteren GUIItem med de items der ligger i modelen. Findes det item der ledes efter sendes det videre
        /// til EditItem controlleren og EditItemViewet bliver vidst. Findes det ikke returneres LisViewet.
        /// </summary>
        /// <param name="itemToEdit"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Funktionen sammenligner parameteren GUIItem med de items der ligger i modelen. Findes det item der ledes efter
        /// bliver det fjernet fra listen med GUIItems. Da listen ligger i en IEnumerable konverteres den til en Liste
        /// hvorefter itemToDelete bliver fjernet fra listen, og den nye liste bliver lagt i model igen.
        /// LisView returneres.
        /// </summary>
        /// <param name="itemToDelete"></param>
        /// <returns></returns>
        public ActionResult DeleteSelectedItem(GUIItem itemToDelete)
        {
            var uow = _dal.GetUnitOfWork();
            foreach (var item in model)
            {
                if (item.Type == itemToDelete.Type && item.Amount == itemToDelete.Amount && item.Size == itemToDelete.Size && item.Unit == itemToDelete.Unit)
                {
                    List<GUIItem> tempList = model.ToList();
                    tempList.Remove(itemToDelete);
                    model = tempList;
                    foreach (var dbitem in _dbItems)
                    {
                        if (dbitem.ItemName == itemToDelete.Type)
                        {
                            uow.ItemRepo.Delete(dbitem);
                        }
                    }
                    foreach (var dblistitem in _dbListItems)
                    {
                        if (dblistitem.ItemId == itemToDelete.Id)
                        {
                            uow.ListItemRepo.Delete(dblistitem);
                        } 
                    }
                }
            }
          _dal.DisposeUnitOfWork();
            return RedirectToAction("ListView", "LisView");
        }
    }
}