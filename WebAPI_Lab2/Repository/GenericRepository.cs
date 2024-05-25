using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI_Lab2.Data;

namespace WebAPI_Lab2.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ItiContext _context;
        private DbSet<T> table = null;

        public GenericRepository(ItiContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }

        public async Task DeleteAsync(object id)
        {
            T existing = await GetByIdAsync(id);
            if (existing != null)
                table.Remove(existing);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await table.FindAsync(id);
        }

        public async Task InsertAsync(T entity)
        {
            await table.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = table;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
    }
}
