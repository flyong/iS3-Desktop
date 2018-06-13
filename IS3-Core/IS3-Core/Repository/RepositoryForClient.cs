using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core
{
    public class RepositoryForClient<T> : IRepository<T> where T : class, new()
    {
        public List<T> BatchCreate(List<T> models)
        {
                throw new NotImplementedException();
        }

        public int BatchDelete(IList<Guid> guids)
        {
            throw new NotImplementedException();
        }

        public List<T> BatchUpdate(List<T> models)
        {
            throw new NotImplementedException();
        }

        public T Create(T model)
        {
            throw new NotImplementedException();
        }

        public int Delete(Guid guid)
        {
            throw new NotImplementedException();
        }

        public int Delete(int key)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(Expression<Func<T, bool>> expression, Expression<Func<T, dynamic>> sortPredicate, SortOrder sortOrder, int skip, int take, out int total)
        {
            throw new NotImplementedException();
        }

        public T Retrieve(Guid guid)
        {
            throw new NotImplementedException();
        }

        public T Retrieve(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T Retrieve(int key)
        {
            throw new NotImplementedException();
        }

        public T Update(T model)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IRepository<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<T> IRepository<T>.Retrieve(int key)
        {
            throw new NotImplementedException();
        }
    }
}
