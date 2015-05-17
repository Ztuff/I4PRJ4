using System;
using System.Data;

namespace DataAccessLayer.AdoNetUoW
{
    /// <summary>
    /// Interface to the Ado.NET Context.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Creates a unit of work for database transactions.
        /// </summary>
        /// <returns></returns>
        IUnitOfWork CreateUnitOfWork();

        /// <summary>
        /// Creates a commands to be executed.
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();
    }
}
