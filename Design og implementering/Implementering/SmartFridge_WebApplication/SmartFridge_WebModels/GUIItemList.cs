using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartFridge_WebApplication.Models
{
    public class GUIItemList
    {
        public int ID { get; private set; }
        public string Name;
        public List<GUIItem> ItemList { get; private set; }

        public GUIItemList(int id, string name)
        {
            Name = name;
            ID = id;
            ItemList = new List<GUIItem>();
        }
    }
}