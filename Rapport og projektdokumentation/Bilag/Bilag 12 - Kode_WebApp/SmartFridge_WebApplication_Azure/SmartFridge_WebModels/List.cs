using System.Collections.Generic;

namespace SmartFridge_WebModels
{
    public class List
    {
        public int ListId { get; set; }
        public string ListName { get; set; }

        public ICollection<ListItem> ListItems { get; set; }

        #region Constructors
        public List()
        {
            
        }

        public List(string listname)
        {
            ListName = listname;
        }

        #endregion
    }
}
