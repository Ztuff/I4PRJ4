using System.Windows;
using System.Windows.Controls;

namespace UserControlLibrary
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
