using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;

namespace SmartFridge_Cache
{
    public class Cache
    {
        /// <summary>
        /// Sets the current list, and loads all the relevant ListItems for this list.
        /// Also loads all Items from the database, since these are used in both
        /// EditItem and AddItem.
        /// </summary>
        public static List CurrentList { get { return _currentList; }
            set
            {
                _currentList = value;
                var uow = DalFacade.GetUnitOfWork();
                CurrentListItems = new List<ListItem>();
                var tempList = uow.ListItemRepo.GetAll().ToList();
                if (tempList.Any())
                {
                    foreach (var Listitem in tempList)
                    {
                        if (Listitem.ListId == _currentList.ListId)
                        {
                            CurrentListItems.Add(Listitem);
                        }
                    }
                    
                }

                DbItems = uow.ItemRepo.GetAll().ToList();

                DalFacade.DisposeUnitOfWork();
            }
        }

        private static List _currentList; 

        public static List<ListItem> CurrentListItems { get; private set; }

        public static List<Item> DbItems { get; private set; } 

        public static ISmartFridgeDALFacade DalFacade { get; set; }

    }
}
