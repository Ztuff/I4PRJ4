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
    public enum GridContent
    {
        ListSelection,
        InFridge,
        ShoppingList,
        StdContent,
        AddItem
    };

    /// <summary>
    /// Interaction logic for CtrlTemplate.xaml
    /// </summary>
    public partial class CtrlTemplate : UserControl
    {

        public CtrlTemplate()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeGridContent(GridContent.ListSelection);
        }

        public void ChangeGridContent(GridContent content)
        {
            switch (content)
            {
                case GridContent.ListSelection:
                    CtrlTempGrid.Children.Clear();
                    CtrlShowListSelection sls = new CtrlShowListSelection(this);
                    CtrlTempGrid.Children.Add(sls);
                    break;

                case GridContent.InFridge:
                case GridContent.ShoppingList:
                case GridContent.StdContent:
                    CtrlTempGrid.Children.Clear();

                    CtrlItemList il = new CtrlItemList(content, this);
                    CtrlTempGrid.Children.Add(il);

                    break;
                case GridContent.AddItem:
                default:
                    throw new Exception();
                    break;
            }
        }
    }
}
