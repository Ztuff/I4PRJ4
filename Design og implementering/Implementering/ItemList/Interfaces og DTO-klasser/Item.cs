using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces_og_DTO_klasser
{
    public class Item
    {
        public Item()
        {
            Amount = 1;
        }

        public string Type { get; set; }
        public uint Amount { get; set; }
        public uint Size { get; set; }
        public string Unit { get; set; }

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
