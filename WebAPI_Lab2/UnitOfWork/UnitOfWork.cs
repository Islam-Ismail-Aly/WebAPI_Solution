using WebAPI_Lab2.Data;
using WebAPI_Lab2.Helpers;
using WebAPI_Lab2.Repository;

namespace WebAPI_Lab2.UnitOfWork
{
    public class UnitOfWork<T> : IDisposable, IUnitOfWork<T> where T : class
    {
        private readonly ItiContext _context;
        private IGenericRepository<T> _entity;
        private readonly IHelperRepository _helperRepository;

        public UnitOfWork(ItiContext context, IHelperRepository helperRepository)
        {
            _context = context;
            _helperRepository = helperRepository;
        }
        public IGenericRepository<T> Entity
        {
            get
            {
                return _entity ?? (_entity = new GenericRepository<T>(_context));
            }
        }

        public IHelperRepository HelperRepository => _helperRepository;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
