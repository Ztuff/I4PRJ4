using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartFridge_WebDAL.Repository
{
    /// <summary>
    /// Generic Repository Pattern interface, containing functions accessing the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);
        /// <summary>
        /// Inserts a collection of the entity type.
        /// </summary>
        /// <param name="entity">Collection of the entity type.</param>
        void AddCollection(ICollection<T> entity);
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="t"></param>
        void Delete(T t);
        /// <summary>
        /// Counts the amount of entities in a datatable.
        /// </summary>
        /// <returns>Amount of entities.</returns>
        int Count();
        /// <summary>
        /// Gets all entities in a datatable.
        /// </summary>
        /// <returns></returns>
        ICollection<T> GetAll();
        /// <summary>
        /// Gets an entity by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(int id);
        /// <summary>
        /// Finds a entity using a LINQ expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The found entity.</returns>
        T Find(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Finds all entities matching the LINQ expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        ICollection<T> FindAll(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Find a entity using a LINQ expression, with an include using a LINQ expression.
        /// </summary>
        /// <param name="include"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        T FindWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression);
        /// <summary>
        /// Finds all entities matching the LINQ expression, with an include using a LINQ expression.
        /// </summary>
        /// <param name="include"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        ICollection<T> FindAllWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression);
    }
}
