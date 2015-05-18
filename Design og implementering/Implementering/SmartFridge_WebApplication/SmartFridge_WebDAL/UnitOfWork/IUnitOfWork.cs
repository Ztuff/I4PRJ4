using SmartFridge_WebDAL.Repository;
using SmartFridge_WebModels;

namespace SmartFridge_WebDAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<List> ListRepo { get; }
        IRepository<ListItem> ListItemRepo { get; }
        IRepository<Item> ItemRepo { get; }

        void SaveChanges();
    }
}
