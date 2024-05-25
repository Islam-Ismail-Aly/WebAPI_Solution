using Microsoft.EntityFrameworkCore;
using WebAPI_Lab2.Data;

namespace WebAPI_Lab2.Helpers
{
    public class GlobalHelper
    {
        private readonly ItiContext _context;

        public GlobalHelper()
        { }

        public GlobalHelper(ItiContext context)
        {
            _context = context;
        }

        public bool Exists<T>(Func<T, bool> predicate) where T : class
        {
            return _context.Set<T>().Any(predicate);
        }

    }
}
