

using System.Linq.Expressions;

namespace E_Commerce_API.GenaricRepo
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : class
    {

        private readonly Application _db;
        private readonly DbSet<T> _Set;

        public GenaricRepo(Application db)
        {
            _db = db; 
            _Set = _db.Set<T>();
        }

        // Search Implement On DB 
        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            // الفلترة بتتم جوه الـ SQL Server وبيرجع بس الداتا المطلوبة
            return await _Set.Where(predicate).ToListAsync(ct);
        }

        public async Task<T> GetByIdWithIncludesAsync(int id, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _Set;

            // بنلف على كل الـ Includes اللي هتبعتها وتزودها على الكويري
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // بنجيب العنصر بالـ Id (افترضنا إن اسم البروبرتي في الـ Entity اسمه Id)
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


        public async Task<IReadOnlyList<T>> GetAll(CancellationToken ct)
        {
             
            return await _Set.ToListAsync(ct);
        }



        public async Task<T> GetByIdAsync(int id, CancellationToken ct)
        {
             return await _Set.FindAsync(id);
        }




        public async Task<T> AddAsync(T value)
        {
            await _Set.AddAsync(value);
            return value;
        }





        public void Update(T value)
        {
            _Set.Update(value);
        }


        public void Delete(T value)
        {
            _Set.Remove(value);
        }

    }
}

