using System.Collections.ObjectModel;
using InterfacesAndDTO;

namespace UserControlLibrary
{
    public interface IItemData
    {
        ObservableCollection<Item> GetData();
    }
}