using System;

namespace DAL.Entities
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

        #region Constructors

        public ListItem()
        {
            
        }

        public ListItem(int amount, int volume, string unit, DateTime? shelfLife)
        {
            Amount = amount;
            Volume = volume;
            Unit = unit;
            ShelfLife = shelfLife;
        }

        public ListItem(int amount, int volume, string unit, DateTime? shelfLife, List list, Item item)
        {
            Amount = amount;
            Volume = volume;
            Unit = unit;
            ShelfLife = shelfLife;
            List = list;
            Item = item;
        }

        #endregion
    }
}
