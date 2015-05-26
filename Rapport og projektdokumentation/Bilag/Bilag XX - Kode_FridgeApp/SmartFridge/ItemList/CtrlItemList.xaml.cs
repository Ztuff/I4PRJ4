using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BusinessLogicLayer;
using InterfacesAndDTO;

namespace UserControlLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CtrlItemList : UserControl
    {
        private CtrlTemplate _ctrlTemp;
        private string ListType;
        private GUIItem selectedItemOld = new GUIItem();
        private GUIItem selectedItem = new GUIItem(); //(GUIItem)DataGridItems.SelectedItem;
        private ObservableCollection<GUIItem> GUIItems;
        private List<string> unitNames = new List<string>();

        /// <summary>
        /// Sets view and local variables
        /// </summary>
        /// <param name="listType"></param>
        /// <param name="ctrlTemp"></param>
        public CtrlItemList(string listType, CtrlTemplate ctrlTemp)
        {
            InitializeComponent();
            _ctrlTemp = ctrlTemp;
            ListType = listType;
            LabelItemList.Content = ListType;
            //  GUIItems = fakeData.GetItemsFromTable(ListType);
            _ctrlTemp._bll.CurrentList = ListType;
            GUIItems = _ctrlTemp._bll.WatchItems;//kan ikke hente data før READ er fikset
            LoadItemData();
            GetUnitNames();
            SelectedUnitCB.ItemsSource = unitNames;
            HideButtonsAndTextboxes();
            DataGridItems.SelectedIndex = 0;
            //DataGridItems.Items
            DataGridItems_SelectionChanged(null, null);
        }
        /// <summary>
        /// Sets data for items in gridview
        /// </summary>
        private void LoadItemData()
        {
            DataGridItems.AutoGenerateColumns = false;
            DataGridItems.CanUserAddRows = false;
            DataGridItems.ItemsSource = GUIItems;
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
            // name.Width = 195;
            DataGridItems.Columns[0].Width = 70;
            DataGridItems.Columns[1].Width = 292;
            DataGridItems.Columns[2].Width = 70;
            DataGridItems.Columns[3].Width = 90;
            DataGridItems.Columns[4].Width = 70;
            //DataGridItems.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; //VIRKER IKKE!




            //LabelItemList.Content =GUIItems[0].ItemType;
        }
        /// <summary>
        /// Loads data for selected item to boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*Item*/
            selectedItem = (GUIItem)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {
                SelectedItemType.Content = selectedItem.Type;
                SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
                SelectedSize.Text = "Størrelse: " + selectedItem.Size.ToString() + " " + selectedItem.Unit;
                if (selectedItem.ShelfLife != null)
                    SelectedBestBeforeTB.SelectedDate = selectedItem.ShelfLife;
                if (selectedItem.ShelfLife.Date.Year == 9999)
                    SelectedBestBeforeTB.SelectedDate = null; //Not a hack!
                
                BtnEdit.Background = new ImageBrush { ImageSource = TryFindResource("ImgEdit") as ImageSource };
                BtnInc.Background = new ImageBrush { ImageSource = TryFindResource("ImgAdd") as ImageSource };
                BtnDec.Background = new ImageBrush { ImageSource = TryFindResource("ImgRemove") as ImageSource };
                HideButtonsAndTextboxes();
            }
            if (selectedItem == null)
            {
                BtnEdit.Background = new ImageBrush { ImageSource = TryFindResource("ImgEditGrayed") as ImageSource };
                BtnInc.Background = new ImageBrush { ImageSource = TryFindResource("ImgAddGrayed") as ImageSource };
                BtnDec.Background = new ImageBrush { ImageSource = TryFindResource("ImgRemoveGrayed") as ImageSource };
            }
        }

        #region Buttons
        /// <summary>
        /// Increments amount 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonInc_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = (GUIItem)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {

                selectedItem = (GUIItem)DataGridItems.SelectedItem;
                selectedItemOld.Type = selectedItem.Type;
                selectedItemOld.Unit = selectedItem.Unit;
                selectedItemOld.Size = selectedItem.Size;
                selectedItemOld.Amount = selectedItem.Amount;
                selectedItemOld.ShelfLife = selectedItem.ShelfLife;

                selectedItem.Amount += 1;

                SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
                _ctrlTemp._bll.ChangeItem(selectedItemOld, selectedItem);
            }
            GUIItems = _ctrlTemp._bll.WatchItems; //Reloader vores guiItems så de passer med DB
            DataGridItems.Items.Refresh();
        }
        /// <summary>
        /// Decrements amount 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = (GUIItem)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {
                selectedItem = (GUIItem)DataGridItems.SelectedItem;
                selectedItemOld.Type = selectedItem.Type;
                selectedItemOld.Unit = selectedItem.Unit;
                selectedItemOld.Size = selectedItem.Size;
                selectedItemOld.Amount = selectedItem.Amount;
                selectedItemOld.ShelfLife = selectedItem.ShelfLife;

                selectedItem.Amount -= 1;

                if (selectedItem.Amount <= 0)
                {
                    _ctrlTemp._bll.DeleteItem(selectedItem); //Hvis det valgte Item rammer 0, og skal slettes
                    GUIItems.Remove(selectedItem);
                    DataGridItems.UnselectAllCells();
                    DataGridItems.Items.Refresh();
                    DataGridItems.SelectedIndex = 0;

                }
                else
                {
                    SelectedAmount.Text = "Antal: " + selectedItem.Amount.ToString();
                    _ctrlTemp._bll.ChangeItem(selectedItemOld, selectedItem);
                }
            }

            GUIItems = _ctrlTemp._bll.WatchItems; //Reloader vores guiItems så de passer med DB
            DataGridItems.Items.Refresh();
        }
        /// <summary>
        /// Shows boxes for editing selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

            selectedItem = (GUIItem)DataGridItems.SelectedItem;
            if (selectedItem != null)
            {
                //selectedItemOld = selectedItem;

                selectedItemOld.Type = selectedItem.Type;
                selectedItemOld.Unit = selectedItem.Unit;
                selectedItemOld.Size = selectedItem.Size;
                selectedItemOld.Amount = selectedItem.Amount;

                SelectedItemTB.Text = selectedItem.Type;
                SelectedAmountTB.Text = selectedItem.Amount.ToString();
                SelectedSizeTB.Text = selectedItem.Size.ToString();
                SelectedUnitTB.Text = selectedItem.Unit;
                ShowButtonsAndTextboxes();
                SelectedItemTB.DataContext = selectedItem.Type;
                SelectedAmountTB.DataContext = selectedItem.Amount;
                SelectedSizeTB.DataContext = selectedItem.Size;
                SelectedUnitTB.DataContext = selectedItem.Unit;
                //SelectedBestBeforeTB.DataContext = selectedItem.BestBefore;

                BtnInc.Opacity = 50;
                BtnInc.IsEnabled = false;
                BtnDec.Opacity = 50;
                BtnDec.IsEnabled = false;
            }
        }
        /// <summary>
        /// Deletes selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

            GUIItem itemDelete = DataGridItems.SelectedItem as GUIItem;
            GUIItems.Remove(itemDelete);
            _ctrlTemp._bll.DeleteItem(itemDelete); //Calling BLL delete through CtrlTemplate
            DataGridItems.UnselectAllCells();
            DataGridItems.SelectedIndex = 0;

        }
        /// <summary>
        /// Hides edit boxes and discards changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedItemTB.Text = selectedItem.Type;
            SelectedAmountTB.Text = selectedItem.Amount.ToString();
            SelectedSizeTB.Text = selectedItem.Size.ToString();
            SelectedUnitTB.Text = selectedItem.Unit;
            BtnInc.Opacity = 100;
            BtnInc.IsEnabled = true;
            BtnDec.Opacity = 100;
            BtnDec.IsEnabled = true;
            HideButtonsAndTextboxes();
        }
        /// <summary>
        /// Controls input values and updates item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            _ctrlTemp._bll.ChangeItem(selectedItemOld, selectedItem); //Calling BLL change through CtrlTemplate
            GUIItems = _ctrlTemp._bll.WatchItems; //Reloader vores guiItems så de passer med DB

            DataGridItems.ItemsSource = GUIItems;
            BtnInc.Opacity = 100;
            BtnInc.IsEnabled = true;
            BtnDec.Opacity = 100;
            BtnDec.IsEnabled = true;
            HideButtonsAndTextboxes();
        }

        #endregion
        /// <summary>
        /// Shows editing input boxes and buttons
        /// </summary>
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
            SelectedBestBeforeTB.Opacity = 100;
            SelectedBestBeforeTB.IsEnabled = true;
            BtnAccept.IsEnabled = true;
            BtnAccept.Opacity = 100;
            BtnCancel.IsEnabled = true;
            BtnCancel.Opacity = 100;
        }
        /// <summary>
        /// Shows input fields and buttons
        /// </summary>
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
            SelectedBestBeforeTB.Opacity = 100;
            SelectedBestBeforeTB.IsEnabled = false;
            BtnAccept.IsEnabled = false;
            BtnAccept.Opacity = 0;
            BtnCancel.IsEnabled = false;
            BtnCancel.Opacity = 0;
        }
        /// <summary>
        /// Sets unit text from dropdown 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedUnitCB_OnDropDownClosed(object sender, EventArgs e)
        {
            if (SelectedUnitCB.SelectedItem != null)
            {
                SelectedUnitTB.Text = SelectedUnitCB.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Returns unit names
        /// </summary>
        private void GetUnitNames()
        {
            unitNames.Add("l");
            unitNames.Add("dl");
            unitNames.Add("cl");
            unitNames.Add("ml");
            unitNames.Add("kg");
            unitNames.Add("g");
        }
        /// <summary>
        /// changes UC to add items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new AddItem(ListType, _ctrlTemp));
        }
    }
}
