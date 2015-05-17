using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridge_WebApplication.Models;
using SmartFridge_WebApplication.DAL.Repository;

namespace SmartFridge_WebApplication.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<List> ListRepo { get; }
        IRepository<ListItem> ListItemRepo { get; }
        IRepository<Item> ItemRepo { get; }

        void SaveChanges();
    }
}
