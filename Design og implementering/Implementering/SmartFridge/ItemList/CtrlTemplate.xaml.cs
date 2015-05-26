using System.Windows.Controls;
using BusinessLogicLayer;

namespace UserControlLibrary
{

    /// <summary>
    /// Logic that controls all UserControls
    /// Interaction logic for CtrlTemplate.xaml
    /// </summary>
    public partial class CtrlTemplate : UserControl
    {
        private UserControl _uc;
        public UserControl[] NavigationHistoryCollection { get; private set; }
        private int NavigationHistoryCollectionPosition;
        private int NavigationHistoryCollectionOriginalPosition;
        public readonly BLL _bll = new BLL();
        /// <summary>
        /// Sets a collection history for navigation. Size = 10
        /// </summary>
        public CtrlTemplate()
        {
            InitializeComponent();

            _uc = new CtrlShowListSelection(this);
            CtrlTempGrid.Children.Add(_uc);

            //ChangeGridContent(_uc);

            NavigationHistoryCollection = new UserControl[10]; //Opretter Navigation History list
            NavigationHistoryCollection[0] = _uc;
            NavigationHistoryCollectionPosition = 0; //Sætter navigationspilen til at pege på start-up siden
            NavigationHistoryCollectionOriginalPosition = NavigationHistoryCollectionPosition;
        }

        /// <summary>
        /// Changes the usercontrol in grid
        /// </summary>
        /// <param name="uc"></param>
        public void ChangeGridContent(UserControl uc)
        {
            _uc = uc;
            CtrlTempGrid.Children.Clear();
            CtrlTempGrid.Children.Add(_uc);

            if (NavigationHistoryCollection[NavigationHistoryCollectionPosition] != uc)
            {
                if (NavigationHistoryCollectionPosition != 9)
                {            
                    NavigationHistoryCollectionPosition += 1;
                }
                else
                {
                    NavigationHistoryCollectionPosition = 0;
                }

                NavigationHistoryCollection[NavigationHistoryCollectionPosition] = _uc;
                NavigationHistoryCollectionOriginalPosition = NavigationHistoryCollectionPosition;   
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
        /// <summary>
        /// loads previous UC
        /// </summary>
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
        /// <summary>
        /// Loads UC navigated back from
        /// </summary>
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
