﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesAndDTO
{
    /// <summary>
    /// Class that symbols all the data the user needs to know about an item
    /// </summary>
    public class GUIItem
    {
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

        public string Type { get; set; }
        public uint Amount { get; set; }
        public uint Size { get; set; }
        public string Unit { get; set; }
        public DateTime ShelfLife { get; set; }
        public int ID { get; set; }

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
