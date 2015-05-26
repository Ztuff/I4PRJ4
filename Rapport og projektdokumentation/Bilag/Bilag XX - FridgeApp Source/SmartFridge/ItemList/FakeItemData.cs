using System.Collections.ObjectModel;
using InterfacesAndDTO;

namespace UserControlLibrary
{
    class FakeItemData : IItemData
    {
        public ObservableCollection<GUIItem> GetData()
        {
            return new ObservableCollection<GUIItem>()
            {
                new GUIItem("Type 1", 1, 1, "g"),
                new GUIItem("Type 2", 2, 1, "kg"),
                new GUIItem("Type 3", 3, 1, "ml"),
                new GUIItem("Type 4", 4, 1, "dl"),
                new GUIItem("Type 5", 5, 1, "l")
            };
        }
    }
}