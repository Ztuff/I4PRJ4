using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer;
using InterfacesAndDTO;

namespace UserControlLibrary
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class AddItem : UserControl
    {
        ObservableCollection<GUIItem> newItems = new ObservableCollection<GUIItem>();
        ObservableCollection<GUIItem> types = new ObservableCollection<GUIItem>();
        private uint amount = 1;
        private string selectedType = "";
        public string _currentList;
        private CtrlTemplate _ctrlTemp;

        /// <summary>
        /// Sets local attributes
        /// </summary>
        /// <param name="currentList"></param>
        /// <param name="ctrlTemp"></param>
        public AddItem(string currentList, CtrlTemplate ctrlTemp)
        {
            _currentList = currentList;
            _ctrlTemp = ctrlTemp;
            InitializeComponent();
            ListBoxItems.ItemsSource = newItems;
            TextBoxAntal.Text = amount.ToString();
            ComboBoxVaretype.ItemsSource = _ctrlTemp._bll.Types;
            ComboBoxUnit.ItemsSource = _ctrlTemp._bll.UnitNames;
        }

        #region OtherMethods

        /// <summary>
        /// returns guiItem from its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private GUIItem GetTypeItemFromName(string name)
        {
            foreach (var item in types)
            {
                if (item.Type.Equals(name))
                    return item;
            }
            return new GUIItem();
        }
        
        /// <summary>
        /// Sets first letter to uppercase in string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// calls create new item when add is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
        }

        /// <summary>
        /// Creates new item based on input fields
        /// </summary>
        /// <returns></returns>
        private GUIItem CreateNewItem()
        {
            if (TextBoxShelfLife.SelectedDate == null)
                TextBoxShelfLife.SelectedDate = new DateTime(9999, 1, 1);
            return _ctrlTemp._bll.CreateNewItem(TextBoxVareType.Text, Convert.ToUInt32(TextBoxAntal.Text),
                Convert.ToUInt32(TextBoxVolumen.Text), TextBoxVolumenEnhed.Text, TextBoxShelfLife.SelectedDate.Value);
        }
        /// <summary>
        /// Adds new item to view and collection
        /// </summary>
        /// <param name="item"></param>
        private void AddNewItem(GUIItem item)
        {
            foreach (var i in newItems)
            {
                if (i.Type.Equals(item.Type) && i.Size == item.Size && i.ShelfLife.Equals(item.ShelfLife))
                {
                    i.Amount += item.Amount;
                    ListBoxItems.Items.Refresh();
                    return;
                }
            }
            newItems.Add(item);

            ListBoxItems.Items.Refresh();
        }
        /// <summary>
        /// Update Text boxes From Type
        /// </summary>
        /// <param name="item"></param>
        private void UpdateTextboxesFromType(GUIItem item)
        {
            if (item != null)
            {
                amount = item.Amount;
                TextBoxAntal.Text = amount.ToString();
                TextBoxVolumen.Text = item.Size.ToString();
                TextBoxVolumenEnhed.Text = item.Unit;
            }
        }

        #endregion

        #region ControlMethods
        /// <summary>
        /// Calls create new item, and exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddExitButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
            Exit();
        }
        /// <summary>
        /// Calls BLL to add all new items and returns to list view
        /// </summary>
        private void Exit()
        {
            _ctrlTemp._bll.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));
        }
        /// <summary>
        /// Increments amount of selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            amount++;
            TextBoxAntal.Text = amount.ToString();
        }
        /// <summary>
        /// Decrements amount of selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            amount--;
            if (amount < 1)
                amount = 1;
            TextBoxAntal.Text = amount.ToString();
        }
        /// <summary>
        /// Sets amount to 1 if it was 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxAntal_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                amount = Convert.ToUInt32(TextBoxAntal.Text);
            }
            catch
            {
                amount = 1;
            }
            finally
            {
                TextBoxAntal.Text = amount.ToString();
            }
        }
        /// <summary>
        /// /// Sets type and checks if it already exists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxVareType_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxVareType.Text = UppercaseFirst(TextBoxVareType.Text);
            var item = GetTypeItemFromName(TextBoxVareType.Text);
            if (item.Type != null)
                UpdateTextboxesFromType(item);
        }
        /// <summary>
        /// Sets type textbox based on selected from dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxVaretype_OnDropDownClosed(object sender, EventArgs e)
        {
            if (ComboBoxVaretype.SelectedItem != null)
            {
                GUIItem temp = (GUIItem)ComboBoxVaretype.SelectedItem;
                selectedType = temp.Type;
                TextBoxVareType.Text = selectedType;
                TextBoxVareType_OnLostFocus(null, null);
                UpdateTextboxesFromType(temp);
            }
        }
        /// <summary>
        /// Sets unit textbox based on selected from dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxUnit_OnDropDownClosed(object sender, EventArgs e)
        {
            if (ComboBoxUnit.SelectedItem != null)
            {
                TextBoxVolumenEnhed.Text = ComboBoxUnit.SelectedItem.ToString();
            }
        }

        #endregion
        /// <summary>
        /// Sets volume to 1 if it was 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxVolumen_OnLostFocus(object sender, RoutedEventArgs e)
        {
            uint volumen = 0;
            try
            {
                volumen = Convert.ToUInt32(TextBoxVolumen.Text);
            }
            catch
            {
                volumen = 1;
            }
            finally
            {
                TextBoxVolumen.Text = volumen.ToString();
            }
        }
    }
}
