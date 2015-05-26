using System;

namespace DataAccessLayer.AdoNetUoW
{
    /// <summary>
    /// Interface for the unit of work class. 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves changes from the transaction.
        /// </summary>
        void SaveChanges();
    }
}
