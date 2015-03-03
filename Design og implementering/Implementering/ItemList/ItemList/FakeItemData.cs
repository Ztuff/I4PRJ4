using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ItemList
{
    class FakeItemData : IItemData
    {
        public ObservableCollection<Item> GetData()
        {
            return new ObservableCollection<Item>()
            {
                new Item("Type 1", 1, 1, "g"),
                new Item("Type 2", 2, 1, "kg"),
                new Item("Type 3", 3, 1, "ml"),
                new Item("Type 4", 4, 1, "dl"),
                new Item("Type 5", 5, 1, "l")
            };
        }
    }
}