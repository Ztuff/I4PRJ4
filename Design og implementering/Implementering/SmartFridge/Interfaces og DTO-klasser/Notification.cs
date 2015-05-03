using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesAndDTO
{
    public class Notification
    {
        public string Message;
        public DateTime TimeOfNotification;
        public int ID;

        public Notification(string message, DateTime time)
        {
            Message = message;
            TimeOfNotification = time;
        }
    }
}
