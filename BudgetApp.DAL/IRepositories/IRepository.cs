using System.Linq.Expressions;

namespace BudgetApp.DAL
{
    public interface IRepository<T>
    {
        public T Add(T toAdd);
        public IEnumerable<T> GetAll();
        public IEnumerable<T> GetAll(int page, int records);
        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func);
        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] joins);
        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func, List<string> joins);
        public void Remove(T toRemove);
        public void Save();
        public void Update(T toUpdate);
    }
}
