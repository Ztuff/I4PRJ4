using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesAndDTO
{
    public interface IData
    {
        void AddItemsToTable(string table, List<Item> items);
        void RemoveItem(string table, Item item);
        List<Item> GetItemsFromTable(string table);
        List<Item> GetTypes(); //Enten skal denne funktion hente fra alle lister, eller også skal vi have en liste for varetyper.
    }
}
