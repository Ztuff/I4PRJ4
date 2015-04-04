using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_SmartFridge.Entities
{
    public class List
    {
        public int ListId { get; set; }
        public string ListName { get; set; }

        public ICollection<ListItem> ListItems { get; set; } 

        public List(string listname)
        {
            ListName = listname;
        }
    }
}
