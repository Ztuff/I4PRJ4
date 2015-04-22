using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_SmartFridge.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int StdVolume { get; set; }
        public string StdUnit { get; set; }

        public ICollection<ListItem> ListItems { get; set; }

        public Item()
        {
            
        }

        public Item(string itemname)
        {
            ItemName = itemname;
        }
    }
}
