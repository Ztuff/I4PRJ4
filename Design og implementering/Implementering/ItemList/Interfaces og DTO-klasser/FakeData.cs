using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InterfacesAndDTO
{
    public class FakeData : IData
    {
        public void AddItemsToTable(string table, List<Item> items)
        {
            foreach (var VARIABLE in items)
            {
                Debug.WriteLine(VARIABLE.ToString() + " added to " + table);
            }
        }

        public void RemoveItem(string table, Item item)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItemsFromTable(string table)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
