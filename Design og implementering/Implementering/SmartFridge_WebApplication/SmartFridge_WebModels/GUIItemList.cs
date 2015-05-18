using System.Collections.Generic;

namespace SmartFridge_WebModels
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