using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //public List<UserControl> NavigationHistoryCollection { get; private set; }
        public UserControl[] NavigationHistoryCollection { get; private set; }
        private int NavigationHistoryCollectionPosition;
        private int NavigationHistoryCollectionOriginalPosition;
        public CtrlTemplate()
        {
            InitializeComponent();

            _uc = new CtrlShowListSelection(this);
            CtrlTempGrid.Children.Add(_uc);

            //ChangeGridContent(_uc);

            NavigationHistoryCollection = new UserControl[10]; //Opretter Navigation History list
            NavigationHistoryCollection[0] = _uc;
            NavigationHistoryCollectionPosition = 0; //Sæter navigationspilen til at pege på start-up siden
        }

        public void ChangeGridContent(UserControl uc)
        {
            _uc = uc;
            CtrlTempGrid.Children.Clear();
            CtrlTempGrid.Children.Add(_uc);

            if (NavigationHistoryCollection[NavigationHistoryCollectionPosition] != uc)
            {
                if (NavigationHistoryCollectionPosition != 9)
                {
                    //NavigationHistoryCollection.Add(_uc);              
                    NavigationHistoryCollectionPosition += 1;
                    NavigationHistoryCollection[NavigationHistoryCollectionPosition] = _uc;              
                    NavigationHistoryCollectionOriginalPosition = NavigationHistoryCollectionPosition;   
                }
                else
                {
                    NavigationHistoryCollectionPosition = 0;
                    NavigationHistoryCollection[NavigationHistoryCollectionPosition] = _uc;
                }
            }

            #region Udkommenteret Kode - STUFF
            // NavigationHistoryCollection[NavigationHistoryCollection.Count-2]
            /*   if (NavigationHistoryCollection == null || NavigationHistoryCollection.Last() == uc || NavigationHistoryCollection[NavigationHistoryCollectionPosition] == uc)
               {
                
               }
             */
            /* var currentContent = (CtrlTempGrid.Children[0] as CtrlItemList).ListType; //Vi kommer altid (forhåbentligt) fra en CtrlItemList, så vi skal bare have at vide hvad den nuværende liste er.
           CtrlTempGrid.Children.Clear();
           var additem = new AddItem.AddItem(currentContent, this);
           CtrlTempGrid.Children.Add(additem);*/
            #endregion
        }

        public void NavigateBack()
        {
            if (NavigationHistoryCollectionPosition == 0 && NavigationHistoryCollectionOriginalPosition != 9 && (NavigationHistoryCollection[9]) != null)
            {
                NavigationHistoryCollectionPosition = 9;
            }
            else if (NavigationHistoryCollectionPosition != 0 && (NavigationHistoryCollectionPosition - 1) != NavigationHistoryCollectionOriginalPosition && (NavigationHistoryCollection[NavigationHistoryCollectionPosition - 1]) != null)
            {
                NavigationHistoryCollectionPosition -= 1;

            }
            else
            {
                return; //Vi er allerede på den sidste plads
            }

            CtrlTempGrid.Children.Clear();
            CtrlTempGrid.Children.Add(NavigationHistoryCollection[NavigationHistoryCollectionPosition]);
        }

        public void NavigateForward()
        {
            if (NavigationHistoryCollectionPosition == 9 && NavigationHistoryCollectionOriginalPosition != 9 && (NavigationHistoryCollection[0]) != null)
            {
                NavigationHistoryCollectionPosition = 0;
            }
            else if (NavigationHistoryCollectionPosition != 9 && (NavigationHistoryCollectionPosition) != NavigationHistoryCollectionOriginalPosition && (NavigationHistoryCollection[NavigationHistoryCollectionPosition + 1]) != null)
            {
                NavigationHistoryCollectionPosition += 1;
            }
            else
            {
                return; //Vi er allerede på den første plads
            }
            CtrlTempGrid.Children.Clear();
            CtrlTempGrid.Children.Add(NavigationHistoryCollection[NavigationHistoryCollectionPosition]);
        }
    }
}
