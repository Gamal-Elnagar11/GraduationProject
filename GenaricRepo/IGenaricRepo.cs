


using System.Linq.Expressions;

namespace E_Commerce_API.GenaricRepo
{
    public interface IGenaricRepo<T> where T : class
    {
        public Task<IReadOnlyList<T>> GetAll(CancellationToken ct);
        public Task<T> GetByIdAsync(int id, CancellationToken ct);
        public Task<T> AddAsync(T value);
        public void Update (T value);
        public void Delete (T value);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct); // for Search implement on DB

        Task<T> GetByIdWithIncludesAsync(int id, CancellationToken cancellationToken ,params Expression<Func<T, object>>[] includes);
    }
}
