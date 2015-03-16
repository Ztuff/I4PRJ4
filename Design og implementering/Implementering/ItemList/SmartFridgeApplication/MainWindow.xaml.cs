using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WpfAnimatedGif;

namespace SmartFridgeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum SyncStatus
        {
            Syncing,
            Synced,
            Desynced,
        }

        private SyncStatus syncStatus = SyncStatus.Synced;

        public CtrlTemplate _ctrlTemp = new CtrlTemplate();
        public CtrlTemplate CtrlTemp = new CtrlTemplate();

        // public GridContent PrevUserControl;
        public MainWindow()
        {
            InitializeComponent();
            ItemListGrid.Children.Add(CtrlTemp);
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

        private void TestButton_Home_Clicked(object sender, RoutedEventArgs e)
        {
            //    PrevUserControl = (GridContent) Enum.Parse(typeof(GridContent), Temp.Name);
            CtrlTemp.ChangeGridContent(new CtrlShowListSelection(CtrlTemp));
        }

        private void TestButton_Back_Clicked(object sender, RoutedEventArgs e)
        {
            CtrlTemp.NavigateBack();
        }

        private void TestButton_Forward_Clicked(object sender, RoutedEventArgs e)
        {
            CtrlTemp.NavigateForward();
        }

        private void SyncButton_OnClick(object sender, RoutedEventArgs e)
        {
            SyncTest();
        }

        private void SyncTest()
        {
            syncStatus = SyncStatus.Syncing;
            ChangeSyncImage();
            Thread thread = new Thread(WaitAndSetToDesynced);
            thread.Start();
        }

        private void WaitAndSetToDesynced()
        {
            Thread.Sleep(2000);
            syncStatus = SyncStatus.Desynced;
            Dispatcher.Invoke(() => { ChangeSyncImage(); });
        }

        private void ChangeSyncImage()
        {
            switch (syncStatus)
            {
                case SyncStatus.Synced:
                    ImageBehavior.SetAnimatedSource(SyncImage, TryFindResource("ImgSyncSucceeded") as ImageSource);
                    break;
                case SyncStatus.Desynced:
                    ImageBehavior.SetAnimatedSource(SyncImage, TryFindResource("ImgSyncFailed") as ImageSource);
                    break;
                case SyncStatus.Syncing:
                    ImageBehavior.SetAnimatedSource(SyncImage, TryFindResource("ImgSyncing") as ImageSource);
                    break;
            }
        }
    }
}
