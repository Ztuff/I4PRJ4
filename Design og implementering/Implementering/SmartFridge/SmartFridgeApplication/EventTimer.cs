using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using InterfacesAndDTO;
using Timer = System.Timers.Timer;

namespace SmartFridgeApplication
{
    public class EventTimer
    {
        private Timer _eTimer;
        //The timer checks every 30 seconds...
        private int _lastHour = DateTime.Now.Hour;
        private MainWindow _owner; //Need this to call back 
        

        public EventTimer(MainWindow owner, int precisionInSeconds)
        {
            _owner = owner;
            _eTimer = new Timer(1000*precisionInSeconds);
            _eTimer.Elapsed += OnTimedEvent;
            _eTimer.Enabled = true;
            //TriggerShelfChecking();
            //TriggerSyncing();
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //If the time is X o'clock sharp, it'll trigger the timer
            if (_lastHour < DateTime.Now.Hour || (_lastHour == 23 && DateTime.Now.Hour == 0))
            {
                _lastHour = DateTime.Now.Hour;
                TriggerShelfChecking();
                TriggerSyncing();
            }
        }

        public void TriggerShelfChecking()
        {
            var fridge = _owner.CtrlTemp._bll.GetList("Køleskab"); //Get the list
            var newNotifications = _owner.CtrlTemp._bll.CheckShelfLife(fridge); //Call the ShelfLife-checking method
            DontAddDuplicates(newNotifications);
            AddNewNotifications(newNotifications);
            _owner.UpdateNotificationsAmount(); //The above methods can add notifications to '_owner', so we'll have to update the amount, so we can show that in the GUI
        }

        private void AddNewNotifications(List<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                _owner.CtrlTemp._bll.Notifications.Add(notification);
            }
        }

        private void DontAddDuplicates(List<Notification> notifications)
        {
            var duplicatesToRemove = new List<Notification>();
            foreach (var newNotifications in notifications)
            {
                foreach (var oldNotifications in _owner.CtrlTemp._bll.Notifications)
                {
                    if (newNotifications.ItemID == oldNotifications.ItemID)
                    {
                        duplicatesToRemove.Add(newNotifications);
                    }
                }
            }

            notifications.RemoveAll(item => duplicatesToRemove.Contains(item));
        }

        public void TriggerSyncing()
        {
            //TBD
        }
    }
}
