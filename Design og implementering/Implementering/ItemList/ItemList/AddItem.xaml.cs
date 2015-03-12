using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using InterfacesAndDTO;
using ItemList;

namespace AddItem
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class AddItem : UserControl
    {
        List<Item> newItems = new List<Item>();
        List<Item> types = new List<Item>();
        List<string> typeNames = new List<string>();
        List<string> unitNames = new List<string>();
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
            GetTypes();
            GetUnitNames();
            ComboBoxVaretype.ItemsSource = typeNames;
            ComboBoxUnit.ItemsSource = unitNames;
        }

        #region OtherMethods

        private Item GetTypeItemFromName(string name)
        {
            foreach (var item in types)
            {
                if (item.Type.Equals(name))
                    return item;
            }
            return new Item();
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

        private void GetTypes()
        {
            //Data-tier call
            //Hent typer fra databasen

            //Tests:
            types.Add(new Item() { Type = "Mælk", Amount = 1, Size = 1, Unit = "L" });
            types.Add(new Item() { Type = "Nutella" });
            types.Add(new Item() { Type = "Kiks" });
            types.Add(new Item() { Type = "Appelsin" });
            types.Add(new Item() { Type = "Banan" });
            types.Add(new Item() { Type = "Citron" });
            types.Add(new Item() { Type = "Dej" });
            types.Add(new Item() { Type = "Estragon" });
            types.Add(new Item() { Type = "Flødeskum" });

            foreach (var VARIABLE in types)
            {
                typeNames.Add(VARIABLE.Type);
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
        }

        private Item CreateNewItem()
        {
            Item item = new Item();
            item.Type = TextBoxVareType.Text;
            item.Amount = Convert.ToUInt32(TextBoxAntal.Text);
            item.Size = Convert.ToUInt32(TextBoxVolumen.Text);
            item.Unit = TextBoxVolumenEnhed.Text;
            return item;
        }

        private void AddNewItem(Item item)
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

        private void UpdateTextboxesFromType(Item item)
        {
            if (item != null)
            {
                amount = item.Amount;
                TextBoxAntal.Text = amount.ToString();
                TextBoxVolumen.Text = item.Size.ToString();
                TextBoxVolumenEnhed.Text = item.Unit;
            }
        }

        private void Exit()
        {
            dataLayer.AddItemsToTable(CurrentList.ToString(), newItems);
            _ctrlTemplate.ChangeGridContent(CurrentList);
        }

        #endregion

        #region ControlMethods

        private void AddExitButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
            Exit();

        }

<<<<<<< HEAD
=======
        private void Exit()
        {
            dataLayer.AddItemsToTable(_currentList, newItems);
            _ctrlTemp.ChangeGridContent(new CtrlItemList(_currentList, _ctrlTemp));
        }

>>>>>>> 4737cec80b439d210f3be1c64bcc3d45eb894299
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
