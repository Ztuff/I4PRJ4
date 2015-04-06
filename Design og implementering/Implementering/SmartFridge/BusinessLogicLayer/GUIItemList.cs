using System.Collections.ObjectModel;
using InterfacesAndDTO;

namespace BusinessLogicLayer
{
    public class GUIItemList
    {
        public int ID { get; private set; }
        public string Name;
        public ObservableCollection<GUIItem> ItemList { get; private set; }

        public GUIItemList(int id, string name)
        {
            Name = name;
            ItemList = new ObservableCollection<GUIItem>();
        }
    }
}