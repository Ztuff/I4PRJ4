using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InterfacesAndDTO;

namespace ItemList
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CtrlItemList : UserControl
    {
        private CtrlTemplate _ctrlTemp;
        public GridContent _listType;
        public CtrlTemplate CtrlTemp { get; set; }
        IData fakeData = new FakeData();        // Vi skal have fjernet fakes fra alt andet end tests - hurtigst muligt!
        ObservableCollection<Item> Items;

        public CtrlItemList(GridContent content, CtrlTemplate ctrlTemp)
        {
            InitializeComponent();
            _ctrlTemp = ctrlTemp;
            _listType = content;
            Items = fakeData.GetItemsFromTable(content.ToString());
            /*
            switch (_listType)
            {
                case GridContent.InFridge:
                    LabelItemList.Content = "I køleskab";
                    break;
                case GridContent.ShoppingList:
                    LabelItemList.Content = "Indkøbsliste";
                    break;
                case GridContent.StdContent:
                    LabelItemList.Content = "Standard-beholdning";
                    break;
                default:
                    throw new Exception();
            }
            */
            LoadItemData();
        }

        private void LoadItemData()
        {
            DataGridItems.AutoGenerateColumns = false;
            DataGridItems.CanUserAddRows = false;
            DataGridItems.ItemsSource = Items;
            DataGridTextColumn name = new DataGridTextColumn();
            name.Header = "Navn";
            name.Binding = new Binding("Type");
            DataGridItems.Columns.Add(name);
            DataGridTextColumn amount = new DataGridTextColumn();
            amount.Header = "Antal";
            amount.Binding = new Binding("Amount");
            DataGridItems.Columns.Add(amount);
            DataGridTextColumn size = new DataGridTextColumn();
            size.Header = "Størrelse";
            size.Binding = new Binding("Size");
            DataGridItems.Columns.Add(size);
            DataGridTextColumn unit = new DataGridTextColumn();
            unit.Header = "Enhed";
            unit.Binding = new Binding("Unit");
            DataGridItems.Columns.Add(unit);
            // name.Width = DataGridItems.ActualWidth - amount.ActualWidth - size.ActualWidth - unit.ActualWidth;
            // Vi ser lige på at lave en dynamisk bredde på Navn
            // Midlertidig løsning:
            name.Width = 195;

            //LabelItemList.Content = Items[0].ItemType;
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item selectedItem = (Item)DataGridItems.SelectedItem;
            SelectedItemType.Content = selectedItem.Type;
            SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
            SelectedSize.Text = "Størrelse: " + selectedItem.Size.ToString() + " " + selectedItem.Unit;
        }

        private void ButtonInc_Click(object sender, RoutedEventArgs e)
        {
            // Kan ikke implementeres endnu, da det skal synkroniseres med den lokale database i samme omgang.
        }

        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            // Kan ikke implementeres endnu, da det skal synkroniseres med den lokale database i samme omgang.
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Skal åbne User Control for den pågældende use case
            _ctrlTemp.ChangeGridContent(GridContent.AddItem);
        }
    }
}
