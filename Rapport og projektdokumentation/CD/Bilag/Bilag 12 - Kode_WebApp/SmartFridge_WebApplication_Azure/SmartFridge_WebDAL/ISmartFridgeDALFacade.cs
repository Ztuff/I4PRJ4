using SmartFridge_WebDAL.UnitOfWork;

namespace SmartFridge_WebDAL
{
    /// <summary>
    /// Facade interface for the DAL.
    /// </summary>
    public interface ISmartFridgeDALFacade
    {
        /// <summary>
        /// Creates a Unit of Work. Only allows one instance, so if UoW is in use, throws exception.
        /// </summary>
        /// <returns></returns>
        IUnitOfWork GetUnitOfWork();
        /// <summary>
        /// Disposes the UnitOfWork. If no UoW was created, throw exception.
        /// </summary>
        void DisposeUnitOfWork();
        /// <summary>
        /// Name of Database for the app.
        /// </summary>
        string DatabaseName { get; set; }
    }
}
