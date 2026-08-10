using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        T Add<T>(T toadd) where T : class;
        List<T> AddRange<T>(List<T> toadd) where T : class;
        IQueryable<T> Getall<T>() where T : class;
        void Save();
        Task SaveAsync();

        IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func) where T : class;
        IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] joins) where T : class;
        IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func, List<string> joins) where T : class;
        void Remove<T>(T toRemove) where T : class;
        void Update<T>(T toUpdate) where T : class;
    }
}
