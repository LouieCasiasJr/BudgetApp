using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BudgetApp.DAL
{
    public class BaseUnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;
        public BaseUnitOfWork(DbContext context)
        {
            _dbContext = context;
        }

        public T Add<T>(T toadd) where T : class
        {
            _dbContext.Set<T>().Add(toadd);
            return toadd;
        }

        public List<T> AddRange<T>(List<T> toadd) where T : class
        {
            _dbContext.Set<T>().AddRange(toadd);
            return toadd;
        }

        public IQueryable<T> Getall<T>() where T : class
        {
            return _dbContext.Set<T>().AsNoTracking();
        }

        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
        }

        public IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func) where T : class
        {
            return _dbContext.Set<T>()
                .AsNoTracking().AsQueryable()
                .Where(func);
        }


        public IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] joins) where T : class
        {
            var tbl = _dbContext.Set<T>().AsNoTracking().Where(func);

            if (joins != null)
            {
                tbl = (joins.Aggregate(tbl, (current, include) =>
                {
                    return current.Include(include);    
                }));
            }

            return tbl;
        }

        public IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> func, List<string> joins) where T : class
        {
            var tbl = _dbContext.Set<T>().AsNoTracking().Where(func);

            foreach (var j in joins)
            tbl = tbl.AsQueryable().Include(j);

            return tbl;
        }

        public void Remove<T>(T toRemove) where T : class
        {
            _dbContext.Set<T>().Remove(toRemove);
        }

        public void Update<T>(T toUpdate) where T : class
        {
            _dbContext.Entry<T>(toUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
