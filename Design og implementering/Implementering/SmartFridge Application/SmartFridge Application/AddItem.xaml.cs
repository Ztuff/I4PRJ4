using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using SmartFridge;

namespace SmartFridge_Application
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class AddItem : UserControl
    {
        List<Item> newItems = new List<Item>();
        List<Item> types = new List<Item>();
        List<string> typeNames = new List<string>();
        private uint amount = 1;
        private string selectedType = "";

        public AddItem()
        {
            InitializeComponent();
            ListBoxItems.ItemsSource = newItems;
            TextBoxAntal.Text = amount.ToString();
            GetTypes();
            ComboBoxVaretype.ItemsSource = typeNames;
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

        #endregion

        #region ControlMethods

        private void AddExitButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(CreateNewItem());
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
            UpdateTextboxesFromType(GetTypeItemFromName(TextBoxVareType.Text));
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

        #endregion
    }
}
