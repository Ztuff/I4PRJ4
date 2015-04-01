using System;
using System.Collections.Generic;
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
        List<GUIItem> newItems = new List<GUIItem>();
        List<GUIItem> types = new List<GUIItem>();
        private uint amount = 1;
        private string selectedType = "";
        public string _currentList;
        public IData dataLayer = new FakeData();
        private CtrlTemplate _ctrlTemp;

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

        private GUIItem GetTypeItemFromName(string name)
        {
            foreach (var item in types)
            {
                if (item.Type.Equals(name))
                    return item;
            }
            return new GUIItem();
        }

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


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
        }

        //Eksempel på BusinessLogicLayer
        private GUIItem CreateNewItem()
        {
            return _ctrlTemp._bll.CreateNewItem(TextBoxVareType.Text, Convert.ToUInt32(TextBoxAntal.Text),
                Convert.ToUInt32(TextBoxVolumen.Text), TextBoxVolumenEnhed.Text);
        }

        private void AddNewItem(GUIItem item)
        {
            foreach (var i in newItems)
            {
                if (i.Type.Equals(item.Type))
                {
                    i.Amount += item.Amount;
                    ListBoxItems.Items.Refresh();
                    return;
                }
            }
            newItems.Add(item);

            ListBoxItems.Items.Refresh();
        }

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

        private void AddExitButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
            Exit();
        }

        private void Exit()
        {
            dataLayer.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            amount++;
            TextBoxAntal.Text = amount.ToString();
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            amount--;
            if (amount < 1)
                amount = 1;
            TextBoxAntal.Text = amount.ToString();
        }

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

        private void TextBoxVareType_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxVareType.Text = UppercaseFirst(TextBoxVareType.Text);
            var item = GetTypeItemFromName(TextBoxVareType.Text);
            if (item.Type != null)
                UpdateTextboxesFromType(item);
        }

        private void ComboBoxVaretype_OnDropDownClosed(object sender, EventArgs e)
        {
            if (ComboBoxVaretype.SelectedItem != null)
            {
                selectedType = ComboBoxVaretype.SelectedItem.ToString();
                TextBoxVareType.Text = selectedType;
                TextBoxVareType_OnLostFocus(null, null);
            }
        }

        private void ComboBoxUnit_OnDropDownClosed(object sender, EventArgs e)
        {
            if (ComboBoxUnit.SelectedItem != null)
            {
                TextBoxVolumenEnhed.Text = ComboBoxUnit.SelectedItem.ToString();
            }
        }

        #endregion

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
