using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfacesAndDTO
{
    public class FakeData : IData
    {
        public void AddItemsToTable(string table, List<Item> items)
        {
            foreach (var VARIABLE in items)
            {
                Debug.WriteLine(VARIABLE.ToString() + " added to list \"" + table + "\"");
            }
        }

        public void RemoveItem(string table, Item item)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Item> GetItemsFromTable(string table)
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

        public ObservableCollection<Item> GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
