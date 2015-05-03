﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using InterfacesAndDTO;
using SmartFridgeApplication;
using UserControlLibrary;
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

        private List<Notification> Notifications = new List<Notification>();

        DispatcherTimer timer = new DispatcherTimer();
        Clock clock = new Clock();
        Popup popup = new Popup();
        StackPanel Panel = new StackPanel();
        
        private SyncStatus syncStatus = SyncStatus.Synced;
        public CtrlTemplate CtrlTemp = new CtrlTemplate();

        void ShelfLifeChecker()
        {
            while (true)
            {
                List<Notification> notifications = new List<Notification>();
                /*
                //var fridge = CtrlTemp._bll.GetList("Køleskab");
                notifactions = CtrlTemp._bll.CheckShelfLife(fridge);
                */
                //Notifications = notifications;
                Dispatcher.Invoke(UpdateNotificationsAmount);

                int timetosleep = 1000 * 60 * 60; //every hour
                Thread.Sleep(timetosleep);
            }
        }

        // public GridContent PrevUserControl;
        public MainWindow()
        {
            InitializeComponent();
            ItemListGrid.Children.Add(CtrlTemp);
            this.DataContext = clock;
            timer.Interval = TimeSpan.FromSeconds(1); //Timer ticks every 1 second
            timer.Tick += timer_Tick; //Event that is called everytime the timer ticks
            timer.Start();
            //DateBlock.DataContext = clock.Date;
            //TimeBlock.DataContext = clock.Time;
            test_add_new_notifications();
            Thread shelfThread = new Thread(ShelfLifeChecker);
            shelfThread.Start();
        }

        void test_add_new_notifications()
        {
            //ONLY FOR TESTING NOTIFICATIONS:
            Notifications.Add(new Notification("Test notification 1", DateTime.Now){ID = 1});
            Notifications.Add(new Notification("Test notification 2", DateTime.Now){ID = 2});
            Notifications.Add(new Notification("Test notification 3", DateTime.Now){ID = 3});
            //ABOVE ONLY FOR TESTING NOTIFICATIONS
        }

        void timer_Tick(object sender, EventArgs e)
        {
            clock.Update();
            DateTime d;

            d = DateTime.Now;
            TestDateLabel.Content = d.Date;
            TestTimeLabel.Content = d.ToLongTimeString();
        }

        private void UpdateNotificationsAmount()
        {
            TextBoxNotifications.Content = Notifications.Count.ToString();
            UpdateNotificationsButton();
        }

        private void UpdateNotificationsButton()
        {
            NotificationsButton.IsEnabled = Notifications.Count != 0;
            if (Notifications.Count == 0)
                popup.IsOpen = false;
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
            Dispatcher.Invoke(ChangeSyncImage);
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

        private void Close_Button_Clicked(object sender, RoutedEventArgs e)
        {
            var Closing = MessageBox.Show("Are you sure you want to close the program?", "Close", MessageBoxButton.YesNo);

            if (Closing == MessageBoxResult.Yes)
                Close();
        }

        private void NotificationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            
            /*
            TextBlock popupText = new TextBlock();
            popupText.Text = "Popup Text";
            popupText.Background = Brushes.LightBlue;
            popupText.Foreground = Brushes.Blue;
            */
            AddNotificationsToPanel(Notifications, Panel);
            popup.Child = Panel;

            popup.PlacementTarget = button;
            popup.IsOpen = true;
        }

        private void AddNotificationsToPanel(List<Notification> notifications, StackPanel panel)
        {
            panel.Children.Clear();


            foreach (var notification in notifications)
            {
                //We want every message to have text, a delete button and a postpone button
                //So we need a nested stackpanel:
                var horizontalStackPanel = new StackPanel();
                horizontalStackPanel.Orientation = Orientation.Horizontal;
                panel.Children.Add(horizontalStackPanel);

                //Display the message:
                var text = new TextBlock();
                text.Text = notification.Message;
                text.Foreground = Brushes.Black;
                text.Background = Brushes.White;
                text.FontSize = 24;
                horizontalStackPanel.Children.Add(text);

                //Add a delete button:
                var del = new Button();
                del.Content = "Slet";
                del.FontSize = 24;
                del.Command = DeleteNotificationCommand;
                del.CommandParameter = notification.ID;
                horizontalStackPanel.Children.Add(del);

                //Add a postpone button:
                var postpone = new Button();
                postpone.Content = "Udskyd";
                postpone.FontSize = 24;
                postpone.IsEnabled = false;
                horizontalStackPanel.Children.Add(postpone);
            }
            panel.Children.Add(new Button { Content = "Luk", FontSize = 24, Command = ClosePopupCommand });
        }

        private Notification NotificationForDeletion = new Notification("", DateTime.Now);

        public ICommand ClosePopupCommand
        {
            get{
                return new RelayCommand(o => ClosePopup());
            }
        }

        public ICommand DeleteNotificationCommand
        {
            get
            {
                return new RelayCommand(DeleteNotification);
            }
        }

        private void ClosePopup()
        {
            popup.IsOpen = false;
        }

        private void DeleteNotification(object parameter)
        {
            int notificationID = (int) parameter;
            var NotificationForDeletion = Notifications.Single(o => o.ID == notificationID);
            Notifications.Remove(NotificationForDeletion);
            AddNotificationsToPanel(Notifications, Panel);
            UpdateNotificationsAmount();
        }

        private void PostPoneNotification()
        {
            throw new NotImplementedException();
        }


    }
}
