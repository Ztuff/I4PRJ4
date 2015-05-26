using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesAndDTO
{
    /// <summary>
    /// Notification class for outdated items
    /// </summary>
    public class Notification
    {
        public string Message; //Message to display to user in GUI
        public DateTime TimeOfNotification; //When the notification was added
        public int ItemID; //Which item this notification is tied to
        public int ID; //Identifier

        public Notification(string message, DateTime time, int itemID)
        {
            Message = message;
            TimeOfNotification = time;
            ItemID = itemID;
        }
    }
}
