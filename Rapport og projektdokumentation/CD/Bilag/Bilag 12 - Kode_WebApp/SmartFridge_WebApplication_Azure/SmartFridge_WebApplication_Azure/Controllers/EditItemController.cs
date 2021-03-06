﻿using System;
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
        /// Ligger den item, der skal redigeres i ViewData, og indlæser alle Items fra cachen, 
        /// samt laver en liste over de mulige enheder, til brug i viewet
        /// </summary>
        /// <param name="oldGuiItem">Det GUIItem, der skal opdateres</param>
        /// <returns>Returnere EditItem viewet</returns>
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
        /// Henter alle input fra viewets inputfelter og opdateret det valgte GUIItem
        /// Eksistere der allerede et tilsvarende GUIItem tælles dettes amount op, og
        /// Den overskydende GUIItem slettes, ellers opdateres GUIItemet blot med de
        /// nye informationer
        /// </summary>
        /// <param name="collection"> En collection af al input data fra Viewet</param>
        /// <returns> Returnere til ListView</returns>
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

            }
            //Hvis der ikke eksisterer et ListItem. Vi burde aldrig komme herned
            return RedirectToAction("ListView", "LisView");
        }

        /// <summary>
        /// Finder den valgte vares enhed i listen over enheder og sætte denne
        /// som den valgte.
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
        /// I tilfælde af at typen er blevet opdateret til en, der ikke eksistere i databasen i forvejen
        /// Bruges denne funktion til at slette det eksisterende ListItem, og indsætte et nyt med en ny
        /// Item, der også sættes ind i databasen.
        /// </summary>
        /// <param name="listItemToDelete">Det Listitem, der skal slettes</param>
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
        /// I tilfælde af at typen opdateres til en allerede eksisterende varetype, der ikke
        /// er på listen, bruges denne funktion til at slette det gamle ListItem, og indsætte
        /// det opdaterede med den nye Item reference.
        /// </summary>
        /// <param name="listItemToDelete">TDet Listitem, der skal slettes</param>
        /// <param name="existingItem">Det i forvejen eksisterende Item i databasen</param>
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