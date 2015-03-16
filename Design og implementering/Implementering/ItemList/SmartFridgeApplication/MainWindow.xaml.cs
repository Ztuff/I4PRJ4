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
                    var img1 = new BitmapImage();
                    img1.BeginInit();
                    img1.UriSource = new Uri("pack://application:,,,/SmartFridgeApplication;component/Images/Sync Succeeded.png");
                    img1.EndInit();
                    ImageBehavior.SetAnimatedSource(SyncImage, img1);
                    break;
                case SyncStatus.Desynced:
                    var img2 = new BitmapImage();
                    img2.BeginInit();
                    img2.UriSource = new Uri("pack://application:,,,/SmartFridgeApplication;component/Images/Sync Failed.png");
                    img2.EndInit();
                    ImageBehavior.SetAnimatedSource(SyncImage, img2);
                    break;
                case SyncStatus.Syncing:
                    var img3 = new BitmapImage();
                    img3.BeginInit();
                    img3.UriSource = new Uri("pack://application:,,,/SmartFridgeApplication;component/Images/Syncing-Small.gif");
                    img3.EndInit();
                    ImageBehavior.SetAnimatedSource(SyncImage, img3);
                    break;
            }
        }

        private void Close_Button_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
