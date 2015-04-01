using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.AdoNetUoW;
using DataAccessLayer.Connection;
using DataAccessLayer.Repository;
using InterfacesAndDTO;

namespace BusinessLogicLayer
{
    public class BLL
    {
        private IConnectionFactory _connectionFactory = new AppConnectionFactory("SmartFridgeConn");
        private IContext _context;
        private ItemRepository _itemRepository;

        public BLL()
        {
            _context = new AdoNetContext(_connectionFactory);
            _itemRepository = new ItemRepository(_context);
        }

        public readonly List<string> UnitNames = new List<string>
        {
            "l",
            "dl",
            "cl",
            "ml",
            "kg",
            "g"
        };

        public List<GUIItem> Types
        {
            get
            {
                List<Item> dbItems = _itemRepository.GetAll().ToList();
                List<GUIItem> guiItems = new List<GUIItem>();

                foreach (var dbItem in dbItems)
                {
                    GUIItem guiItem = new GUIItem();

                    guiItem.Type = dbItem.ItemName;
                    guiItem.Unit = dbItem.StdUnit;
                    guiItem.Size = (uint)dbItem.StdVolume;

                    guiItems.Add(guiItem);
                }

                return guiItems;
            }
        }

        public GUIItem CreateNewItem(string type, uint amount, uint size, string unit)
        {
            GUIItem item = new GUIItem();
            item.Type = type;
            item.Amount = amount;
            item.Size = size;
            item.Unit = unit;
            return item;
        }

        public void DeleteItem(GUIItem GUIitemToDelete)
        {
            /*Henter alle items fra databasen, da der ikke er nogen direkte måde at connecte
                et GUIitem med et dbItem, da GUIitem ikke har noget ID*/
            List<Item> dbItems = _itemRepository.GetAll().ToList();
            Item dbItemToDelete = new Item();
            /*Finder det dbItem der svarer til det GUIitem der skal fjernes*/
            foreach (var VARIABLE in dbItems)
            {
                if (VARIABLE.ItemName == GUIitemToDelete.Type
                    && VARIABLE.StdUnit == GUIitemToDelete.Unit
                    && (uint) VARIABLE.StdVolume == GUIitemToDelete.Size)
                {
                    dbItemToDelete = VARIABLE;
                    break;
                }
            }
            /*Fjerner det ønskede item fra databasen*/
                _itemRepository.Delete(dbItemToDelete);
        }
    }
}
