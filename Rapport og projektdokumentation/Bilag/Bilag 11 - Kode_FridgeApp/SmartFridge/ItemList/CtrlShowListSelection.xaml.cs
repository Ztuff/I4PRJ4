using System;
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
        
        /// <summary>
        /// Sets the ctrl template and loads view
        /// </summary>
        /// <param name="ctrlTemp"></param>
        public CtrlShowListSelection(CtrlTemplate ctrlTemp)
        {
            try
            {
            InitializeComponent();
            _ctrlTemp = ctrlTemp;
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
        /// <summary>
        /// Calls Item list with the selected list name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInFridge_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("Køleskab", _ctrlTemp));
        }
        /// <summary>
        /// Calls Item list with the selected list name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShoppingList_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("Indkøbsliste", _ctrlTemp));
        }
        /// <summary>
        /// Calls Item list with the selected list name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStdContent_Click(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlItemList("Standard-beholdning", _ctrlTemp));
        }
    }
}
