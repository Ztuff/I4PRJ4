using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace SmartFridgeApplication
{
    public class Clock : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string date;
        string time;

        public Clock()
        {
            Update();
        }

        public void Update()
        {
            Date = DateTime.Now.ToLongDateString();
            Time = DateTime.Now.ToLongTimeString();
            Debug.WriteLine(Date);
            Debug.WriteLine(Time);
        }

        public string Date
        {
            get { return date; }
            private set
            {
                if (date != value)
                {
                    date = value;
                    Notify("Date");
                }
            }
        }

        public string Time
        {
            get { return time; }
            private set
            {
                if (time != value)
                {
                    time = value;
                    Notify("Time");                   
                }
            }
        }

        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        
    }
}
