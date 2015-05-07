using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_SmartFridge.Entities
{
    public class ListItem
    {
        public int Amount { get; set; }
        public int Volume { get; set; }
        public string Unit { get; set; }
        public DateTime? ShelfLife { get; set; }

        public List List { get; set; }
        public int ListId { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }

        public ListItem()
        {
            
        }

        public ListItem(int amount, int volume, string unit)
        {
            Amount = amount;
            Volume = volume;
            Unit = unit;
        }

        public ListItem(int amount, int volume, string unit, List list, Item item)
        {
            Amount = amount;
            Volume = volume;
            Unit = unit;
            List = list;
            Item = item;
        }
    }
}
