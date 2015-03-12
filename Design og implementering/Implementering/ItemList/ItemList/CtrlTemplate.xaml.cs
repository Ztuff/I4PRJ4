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
        private UserControl _uc;
        public CtrlTemplate()
        {
            InitializeComponent();
            _uc = new CtrlShowListSelection(this);
            CtrlTempGrid.Children.Add(_uc);
        }

        public void ChangeGridContent(UserControl uc)
        {
            _uc = uc;
            CtrlTempGrid.Children.Clear();
            CtrlTempGrid.Children.Add(_uc);


                   /* var currentContent = (CtrlTempGrid.Children[0] as CtrlItemList).ListType; //Vi kommer altid (forhåbentligt) fra en CtrlItemList, så vi skal bare have at vide hvad den nuværende liste er.
                    CtrlTempGrid.Children.Clear();
                    var additem = new AddItem.AddItem(currentContent, this);
                    CtrlTempGrid.Children.Add(additem);*/
        }
    }
}
