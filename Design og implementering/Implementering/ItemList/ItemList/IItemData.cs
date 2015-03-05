using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using InterfacesAndDTO;

namespace ItemList
{
    public interface IItemData
    {
        ObservableCollection<Item> GetData();
    }
}