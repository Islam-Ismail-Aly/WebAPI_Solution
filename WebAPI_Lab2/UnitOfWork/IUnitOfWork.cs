using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI_Lab2.Repository;

namespace WebAPI_Lab2.UnitOfWork
{
    public interface IUnitOfWork<T> where T : class
    {
        IGenericRepository<T> Entity { get; }
        IHelperRepository HelperRepository { get; }
        Task SaveAsync();
    }
}
