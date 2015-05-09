using System.Collections.Generic;

namespace DAL.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int StdVolume { get; set; }
        public string StdUnit { get; set; }

        public ICollection<ListItem> ListItems { get; set; }

        #region Constructors
        public Item()
        {
            
        }

        public Item(string itemname)
        {
            ItemName = itemname;
        }
        #endregion
    }
}
