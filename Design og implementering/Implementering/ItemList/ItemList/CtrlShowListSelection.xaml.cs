using System;
using System.Collections.Generic;
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

namespace ItemList
{
    /// <summary>
    /// Interaction logic for ShowListSelectionControl.xaml
    /// </summary>
    public partial class CtrlShowListSelection : UserControl
    {
        private CtrlTemplate _ctrlTemp;
        

        public CtrlShowListSelection(CtrlTemplate ctrlTemp)
        {
            InitializeComponent();
            _ctrlTemp = ctrlTemp;
        }

        private void BtnInFridge_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("I køleskab", _ctrlTemp));
        }

        private void BtnShoppingList_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("Indkøbsliste", _ctrlTemp));
        }

        private void BtnStdContent_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("Standard-beholdning", _ctrlTemp));
        }
    }
}
