using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected SFContext Context;
        protected DbSet<T> DbSet; 

        public Repository(SFContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void AddCollection(ICollection<T> entity)
        {
            DbSet.AddRange(entity);
        }

        public void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public ICollection<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public T Get(int id)
        {
            return DbSet.Find(id);
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return DbSet.SingleOrDefault(expression);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).ToList();
        }

        public T FindWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression)
        {
            return DbSet.Include(include).SingleOrDefault(expression);
        }

        public ICollection<T> FindAllWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression)
        {
            return DbSet.Include(include).Where(expression).ToList();
        }
    }
}
