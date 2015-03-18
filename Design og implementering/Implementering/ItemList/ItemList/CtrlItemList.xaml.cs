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
        public string ListType;
        public CtrlTemplate CtrlTemp { get; set; }
        IData fakeData = new FakeData();        // Vi skal have fjernet fakes fra alt andet end tests - hurtigst muligt!
        Item selectedItem = new Item(); //(Item)DataGridItems.SelectedItem;
        ObservableCollection<Item> Items;
        List<string> unitNames = new List<string>();

        public CtrlItemList(string listType, CtrlTemplate ctrlTemp)
        {
            InitializeComponent();
            _ctrlTemp = ctrlTemp;
            ListType = listType;
            LabelItemList.Content = ListType;
            Items = fakeData.GetItemsFromTable(ListType);
            LoadItemData();
            GetUnitNames();
            SelectedUnitCB.ItemsSource = unitNames;
            HideButtonsAndTextboxes();
            DataGridItems.SelectedIndex = 0;
            //DataGridItems.Items
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
            /*Item*/
            selectedItem = (Item)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {
                SelectedItemType.Content = selectedItem.Type;
                SelectedItemTB.Text = selectedItem.Type;
                SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
                SelectedAmountTB.Text = selectedItem.Amount.ToString();
                SelectedSize.Text = "Størrelse: " + selectedItem.Size.ToString() + " " + selectedItem.Unit;
                SelectedSizeTB.Text = selectedItem.Size.ToString();
                SelectedUnitTB.Text = selectedItem.Unit;
                HideButtonsAndTextboxes();

            }
        }

        private void ButtonInc_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                // Kan ikke implementeres endnu, da det skal synkroniseres med den lokale database i samme omgang.
                selectedItem.Amount += 1;
                SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
            }
            DataGridItems.Items.Refresh();
        }

        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                // Kan ikke implementeres endnu, da det skal synkroniseres med den lokale database i samme omgang.
                selectedItem.Amount -= 1;
                if (selectedItem.Amount <= 0)
                {
                    Items.Remove(selectedItem);
                    DataGridItems.UnselectAllCells();
                    DataGridItems.Items.Refresh();
                    DataGridItems.SelectedIndex = 0;
                }
                else
                SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
            }
            
            DataGridItems.Items.Refresh();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Skal åbne User Control for den pågældende use case
            //_ctrlTemp.ChangeGridContent(new AddItem.AddItem(ListType, _ctrlTemp)); Skal implementeres et andet sted!
            /*Item*/
            selectedItem = (Item)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {
                ShowButtonsAndTextboxes();
                SelectedItemTB.DataContext = selectedItem.Type;
                SelectedAmountTB.DataContext = selectedItem.Amount;
                SelectedSizeTB.DataContext = selectedItem.Size;
                SelectedUnitTB.DataContext = selectedItem.Unit;
                //SelectedBestBeforeTB.DataContext = selectedItem.BestBefore;
            }
        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Item itemDelete = DataGridItems.SelectedItem as Item;
            Items.Remove(itemDelete);
            DataGridItems.UnselectAllCells();
            DataGridItems.SelectedIndex = 0;
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            HideButtonsAndTextboxes();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            foreach (char c in SelectedAmountTB.Text)
            {
                if (c < '0' || c > '9')
                {
                    MessageBox.Show("Ugyldigt input i 'Antal'");
                    return;
                }
            }

            foreach (char c in SelectedSizeTB.Text)
            {
                if (c < '0' || c > '9')
                {
                    MessageBox.Show("Ugyldigt input i 'Størelse'");
                    return;
                }
            }
            selectedItem.Type = SelectedItemTB.Text;
            selectedItem.Amount = Convert.ToUInt32(SelectedAmountTB.Text);
            selectedItem.Size = Convert.ToUInt32(SelectedSizeTB.Text);
            selectedItem.Unit = SelectedUnitTB.Text;
            SelectedItemType.Content = selectedItem.Type;
            SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
            SelectedSize.Text = "Størrelse: " + selectedItem.Size.ToString() + " " + selectedItem.Unit;
            DataGridItems.Items.Refresh();

            HideButtonsAndTextboxes();
        }


        private void ShowButtonsAndTextboxes()
        {
            SelectedItemTB.IsReadOnly = false;
            SelectedItemTB.Opacity = 100;
            SelectedAmountTB.IsReadOnly = false;
            SelectedAmountTB.Opacity = 100;
            SelectedSizeTB.IsReadOnly = false;
            SelectedSizeTB.Opacity = 100;
            SelectedUnitTB.IsReadOnly = false;
            SelectedUnitTB.Opacity = 100;
            SelectedUnitCB.IsReadOnly = false;
            SelectedUnitCB.Opacity = 100;
            SelectedBestBeforeTB.IsReadOnly = false;
            SelectedBestBeforeTB.Opacity = 100;
            SelectedBestBeforeTB.Opacity = 0;
            BtnAccept.IsEnabled = true;
            BtnAccept.Opacity = 100;
            BtnCancel.IsEnabled = true;
            BtnCancel.Opacity = 100;
        }

        private void HideButtonsAndTextboxes()
        {
            SelectedItemTB.IsReadOnly = true;
            SelectedItemTB.Opacity = 0;
            SelectedAmountTB.IsReadOnly = true;
            SelectedAmountTB.Opacity = 0;
            SelectedSizeTB.IsReadOnly = true;
            SelectedSizeTB.Opacity = 0;
            SelectedUnitTB.IsReadOnly = true;
            SelectedUnitTB.Opacity = 0;
            SelectedUnitCB.IsReadOnly = true;
            SelectedUnitCB.Opacity = 0;
            SelectedBestBeforeTB.IsReadOnly = true;
            SelectedBestBeforeTB.Opacity = 0;
            BtnAccept.IsEnabled = false;
            BtnAccept.Opacity = 0;
            BtnCancel.IsEnabled = false;
            BtnCancel.Opacity = 0;
        }


        private void SelectedUnitCB_OnDropDownClosed(object sender, EventArgs e)
        {
            if (SelectedUnitCB.SelectedItem != null)
            {
                SelectedUnitTB.Text = SelectedUnitCB.SelectedItem.ToString();
            }
        }

        private void GetUnitNames()
        {
            unitNames.Add("L");
            unitNames.Add("DL");
            unitNames.Add("CL");
            unitNames.Add("ML");
            unitNames.Add("KG");
            unitNames.Add("G");
        }




    }
}
