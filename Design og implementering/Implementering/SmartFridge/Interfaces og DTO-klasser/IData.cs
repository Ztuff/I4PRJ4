using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesAndDTO
{
    public interface IData
    {
        void AddItemsToTable(string table, List<GUIItem> items);
        void RemoveItem(string table, GUIItem item);
        ObservableCollection<GUIItem> GetItemsFromTable(string table);
        ObservableCollection<GUIItem> GetTypes(); //Enten skal denne funktion hente fra alle lister, eller også skal vi have en liste for varetyper.
    }
}
