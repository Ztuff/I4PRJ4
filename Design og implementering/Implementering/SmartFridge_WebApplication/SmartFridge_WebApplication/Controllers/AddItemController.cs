using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartFridge_WebApplication.DAL;
using SmartFridge_WebApplication.DAL.Context;
using SmartFridge_WebApplication.DAL.UnitOfWork;
using SmartFridge_WebApplication.Models;

namespace SmartFridge_WebApplication.Controllers
{
    public class AddItemController : Controller
    {
        //
        // GET: /AddItem/
        public string currentList;
        private int currentListID; //CurrentListID skal komme et sted fra

        private List<GUIItem> newGuiItems = new List<GUIItem>();
        private List<SelectListItem> ListGuiItemTypes = new List<SelectListItem>();
        private IEnumerable<GUIItem> model;
        private List<Item> ListItemTypes = new List<Item>();

        private ISmartFridgeDALFacade dalFacade = new SmartFridgeDALFacade("SmartFridge-SSDT2");
        private SFContext dbContext = new SFContext("SmartFridge-SSDT2");

        public ActionResult AddItem()
        {
            currentList = TempData["CurrentListToEdit"].ToString();
            var uow = dalFacade.GetUnitOfWork();
            //Test
            //ListItemTypes.Add(new Item("Is"));
            //ListItemTypes.Add(new Item("Vingummi"));
            //ListItemTypes.Add(new Item("Chokolade"));
            //ListItemTypes.Add(new Item("Kage"));
            //ListItemTypes = uow.ItemRepo.GetAll(); //Apparently not legal to do

            foreach (var GuiItemTypes in uow.ItemRepo.GetAll()) 
            {
                ListGuiItemTypes.Add(new SelectListItem { Text = GuiItemTypes.ItemName });
            }
            model = newGuiItems;
            ViewBag.ListNewGuiItems = ListGuiItemTypes;

            dalFacade.DisposeUnitOfWork();
            return View(model);
        }
        #region interne funktioner_OLD
      /*  public void AddGuiItem(string type, uint amount, uint size, string unit)
        {
         
            newGuiItems.Add(new GUIItem(type,amount,size,unit));
        }

        public List<GUIItem> AddAndExit(string type, uint amount, uint size, string unit)
        {
            newGuiItems.Add(new GUIItem(type, amount, size, unit));
            
            return newGuiItems;
        }*/
        #endregion

        [HttpPost]
        public ActionResult addNewItem(string Varetype, string Antal, string Volume, string Enhed, DateTime Holdbarhedsdato)
        {
            GUIItem guiItemToAdd = new GUIItem();
            guiItemToAdd.ShelfLife = Holdbarhedsdato; //AMOUNT READ FROM FIELD
            guiItemToAdd.Amount = Convert.ToUInt32(Antal); //Antal READ FROM FIELD
            guiItemToAdd.Size = Convert.ToUInt32(Volume); //Volume READ FROM FIELD
            guiItemToAdd.Type = Varetype; //Varetype READ FROM FIELD
            guiItemToAdd.Unit = Enhed; //unit READ FROM FIELD

            foreach (var i in newGuiItems)
            {
                if (i.Type.Equals(guiItemToAdd.Type) &&
                    i.Size.Equals(guiItemToAdd.Size) &&
                    i.Unit.Equals(guiItemToAdd.Unit) &&
                    i.ShelfLife.Equals(guiItemToAdd.ShelfLife))
                {
                    i.Amount += guiItemToAdd.Amount;
                    //ListBoxItems.Items.Refresh();
                    return null;//Viewet skal dog opdateres først
                }
            }
            newGuiItems.Add(guiItemToAdd);
            //test
            //newGuiItems.Add(new GUIItem("Tester", 1, 6, "l"));
             //ListBoxItems.Items.Refresh();
             model = newGuiItems;
            ViewBag.ListNewGuiItems = ListGuiItemTypes;
             return View("~/Views/AddItem/AddItem.cshtml",model); //Viewet skal dog opdateres først

            #region FromWPF             

            /*foreach (var i in newItems)
            {
                if (i.Type.Equals(item.Type))
                {
                    i.Amount += item.Amount;
                    ListBoxItems.Items.Refresh();
                    return;
                }
            }
            newItems.Add(item);

            ListBoxItems.Items.Refresh();*/

            #endregion
        }


        [HttpPost]
        public ActionResult addItemAndExit()
        {
            AddItem();
            Exit();
            return null;

            #region FromWPF

            /*AddNewItem(CreateNewItem());
            Exit();*/

            #endregion


        }
        [HttpPost]
        public ActionResult Exit()
        {
            if (newGuiItems.Count == 0) {return null;} //Hvis der ikke er tilføjet nogle items, returnerer vi

            //Kobel de tilføjede items fra listen til databasen
            var uow = dalFacade.GetUnitOfWork();
            foreach (var newGuiItem in newGuiItems)
            {
                ListItem dbListItem = new ListItem();
                Item dbItem = new Item(newGuiItem.Type);

               dbListItem.ShelfLife = newGuiItem.ShelfLife;
               dbListItem.Amount = (int)newGuiItem.Amount;
               dbListItem.Volume = (int)newGuiItem.Size;
               dbListItem.Unit = newGuiItem.Unit;

                dbListItem.ListId = currentListID; 
                dbListItem.Item = dbItem;   //Der skal kontrolleres at en udgave af dbItem ikke eksiterer i DB. Hvis der gør, skal ListItem.Item være lig denne.
                
                uow.ListItemRepo.Add(dbListItem);
                uow.ItemRepo.Add(dbItem);   //Der mangler et check på om Item allerede eksisterer i DB
            }




            dalFacade.DisposeUnitOfWork();
            return View("~/Views/LisView/ListView.cshtml");

            #region FromWPF

            /*_ctrlTemp._bll.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));*/

            #endregion


        }






        /*private GUIItem CreateNewItem()
        {
            
            return _ctrlTemp._bll.CreateNewItem(TextBoxVareType.Text, Convert.ToUInt32(TextBoxAntal.Text),
                Convert.ToUInt32(TextBoxVolumen.Text), TextBoxVolumenEnhed.Text, TextBoxShelfLife.SelectedDate);
        }*/
        //public ActionResult SelectType()
        //{
        //                ViewBag.ListNewGuiItems = newGuiItems;
        //                return View();

        //}
 





    }
}
