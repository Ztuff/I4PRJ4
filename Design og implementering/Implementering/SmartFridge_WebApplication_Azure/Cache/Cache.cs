using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridge_WebDAL;
using SmartFridge_WebModels;

namespace SmartFridge_Cache
{
    /// <summary>
    /// Cache opgave er at indeholde den data der går igen for hvert view og indeholder derfor kun statiske attributter. 
    /// Dette er foruden alle de pågældende ListItems,List og Items tilkoblet denne liste, også en statisk udgave af 
    /// ISmartFridgeDALFacade som samtlige controllers skal gøre brug af, for at få adgang til databasen.
    /// </summary>
    public class Cache
    {
        /// <summary>
        /// CurrentList indeholder den liste der er valgt.
        /// Når den bliver sat, hentes alle ListItems tilkoblet denne liste, og referencer til disse gemmes i CurrentListItems.
        /// Alle Items hentes ligeledes fra databasen.
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
