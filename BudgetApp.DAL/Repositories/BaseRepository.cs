using System.Linq.Expressions;

namespace BudgetApp.DAL
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected IUnitOfWork _UoW;
        public BaseRepository(IUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public T Add(T toAdd)
        {
            return _UoW.Add(toAdd);
        }

        public IEnumerable<T> GetAll()
        {
            return _UoW.Getall<T>();
        }

        public IEnumerable<T> GetAll(int page, int records)
        {
            return _UoW.Getall<T>().Skip(page * records).Take(records);
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func)
        {
            return _UoW.GetWhere<T>(func);
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] joins)
        {
            return _UoW.GetWhere(func, joins);
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> func, List<string> joins)
        {
            return _UoW.GetWhere(func, joins);
        }

        public void Remove(T toRemove)
        {
            _UoW.Remove<T>(toRemove);
        }
        
        public void Save()
        {
            _UoW.Save();
        }

        public void Update(T toUpdate)
        {
            _UoW.Update(toUpdate);
        }

        //public IEnumerable<T> GetSequence(List<string> attributes, List<string> values)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
