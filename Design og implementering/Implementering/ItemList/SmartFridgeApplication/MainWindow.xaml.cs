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
using ItemList;

namespace SmartFridgeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CtrlTemplate _ctrlTemp = new CtrlTemplate();
       // public GridContent PrevUserControl;
        public MainWindow()
        {
            InitializeComponent();
            ItemListGrid.Children.Add(_ctrlTemp);
        }

        private void BackButton_Clicked(object sender, RoutedEventArgs e)
        {
            
           // Temp.ChangeGridContent(GridContent.InFridge);
            //var currentContent = (ItemListGrid.Children[0] as CtrlItemList)._listType; //Vi kommer altid (forhåbentligt) fra en CtrlItemList, så vi skal bare have at vide hvad den nuværende liste er.
            //ItemListGrid.Children.Clear();
            //var additem = new AddItem.AddItem(this, currentContent);
            //ItemListGrid.Children.Add(additem);
            //Temp.ChangeGridContent(Temp.Parent);
        }

        private void TestButton_Clicked(object sender, RoutedEventArgs e)
        {
        //    PrevUserControl = (GridContent) Enum.Parse(typeof(GridContent), Temp.Name);
            _ctrlTemp.ChangeGridContent(new CtrlShowListSelection(_ctrlTemp));
        }

        private void TestButtonForward_Clicked(object sender, RoutedEventArgs e)
        {
          //  Temp.ChangeGridContent(PrevUserControl);
        }

        private void HomeClicked(object sender, RoutedEventArgs e)
        {
            _ctrlTemp.ChangeGridContent(new CtrlShowListSelection(_ctrlTemp));

        }
    }
}
