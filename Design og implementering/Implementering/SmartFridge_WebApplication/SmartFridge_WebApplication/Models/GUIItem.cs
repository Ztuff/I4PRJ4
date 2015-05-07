using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFridge_WebApplication.Models
{
    public class GUIItem
    {
        public string Type { get; set; }
        public uint Amount { get; set; }
        public uint Size { get; set; }
        public string Unit { get; set; }
        public DateTime ShelfLife { get; set; }
        public int ID { get; set; }

        public GUIItem()
        {
            Amount = 1;
        }

        public GUIItem(string type, uint amount, uint size, string unit)
        {
            Type = type;
            Amount = amount;
            Size = size;
            Unit = unit;
        }

        public override string ToString()
        {
            string str = "";
            str += Type;
            str += " Antal: " + Amount;
            str += " Enhed: " + Size + " " + Unit;
            return str;
        }
    }

}
