using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddCollection(ICollection<T> entity);
        void Update(T entity);
        void Delete(T t);
        int Count();
        ICollection<T> GetAll();
        T Get(int id);
        T Find(Expression<Func<T, bool>> expression);
        ICollection<T> FindAll(Expression<Func<T, bool>> expression);
        T FindWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression);
        ICollection<T> FindAllWithInclude(Expression<Func<T, object>> include, Expression<Func<T, bool>> expression);
    }
}
