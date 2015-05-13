using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFridge_WebApplication.Models;
using DAL.Repository;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<List> ListRepo { get; }
        IRepository<ListItem> ListItemRepo { get; }
        IRepository<Item> ItemRepo { get; }

        void SaveChanges();
    }
}
