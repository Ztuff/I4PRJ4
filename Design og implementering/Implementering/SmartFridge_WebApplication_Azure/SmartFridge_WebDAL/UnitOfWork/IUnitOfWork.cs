using SmartFridge_WebDAL.Repository;
using SmartFridge_WebModels;

namespace SmartFridge_WebDAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work Interface, using the IRepository interface.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Repository for the List entity.
        /// </summary>
        IRepository<List> ListRepo { get; }
        /// <summary>
        /// Repository for the ListItem entity.
        /// </summary>
        IRepository<ListItem> ListItemRepo { get; }
        /// <summary>
        /// Repository for the Item entity.
        /// </summary>
        IRepository<Item> ItemRepo { get; }
        /// <summary>
        /// Saves changes made in the Unit of Work.
        /// </summary>
        void SaveChanges();
    }
}
