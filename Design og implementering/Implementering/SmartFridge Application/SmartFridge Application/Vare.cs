using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFridge
{
    public class Item
    {
        private string _type;
        private uint _amount;
        private uint _size;
        private string _unit;

        public string Type { get { return _type; } }
        public uint Amount { get { return _amount; } }
        public uint Size { get { return _size; } }
        public string Unit { get { return _unit; } }
    }
}
