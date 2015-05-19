using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using SmartFridge_WebDAL.Context;

namespace SmartFridge_WebDAL.Repository
{
    /// <summary>
    /// Implements the generic Repository Pattern class, using a Entity Framework DbContext.
    /// </summary>
    /// <typeparam name="T">Entity class.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected SFContext Context;
        protected DbSet<T> DbSet; 

        /// <summary>
        /// Injects the SFContext.
        /// </summary>
        /// <param name="context"></param>
        public Repository(SFContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// Inserts a collection of the entity type.
        /// </summary>
        /// <param name="entity"></param>
        public void AddCollection(ICollection<T> entity)
        {
            DbSet.AddRange(entity);
        }

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// Gets all entities in a datatable.
        /// </summary>
        /// <returns>Amount of entities.</returns>
        public int Count()
        {
            return DbSet.Count();
        }

        /// <summary>
        /// Gets all entities in a datatable.
        /// </summary>
        /// <returns></returns>
        public ICollection<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        /// <summary>
        /// Gets an entity by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// Finds a entity using a LINQ expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The found entity.</returns>
        public T Find(Expression<Func<T, bool>> expression)
        {
            return DbSet.SingleOrDefault(expression);
        }

        /// <summary>
        /// Finds all entities matching the LINQ expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ICollection<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).ToList();
        }

        /// <summary>
        /// Find a entity using a LINQ expression, with an include using a LINQ expression.
        /// </summary>
        /// <param name="include"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T FindWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression)
        {
            return DbSet.Include(include).SingleOrDefault(expression);
        }

        /// <summary>
        /// Finds all entities matching the LINQ expression, with an include using a LINQ expression.
        /// </summary>
        /// <param name="include"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ICollection<T> FindAllWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression)
        {
            return DbSet.Include(include).Where(expression).ToList();
        }
    }
}
