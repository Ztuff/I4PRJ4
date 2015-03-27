using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfacesAndDTO
{
    public class FakeData : IData
    {
        public void AddItemsToTable(string table, List<GUIItem> items)
        {
            foreach (var VARIABLE in items)
            {
                Debug.WriteLine(VARIABLE.ToString() + " added to list \"" + table + "\"");
            }
        }

        public void RemoveItem(string table, GUIItem item)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<GUIItem> GetItemsFromTable(string table)
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

        public ObservableCollection<GUIItem> GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
