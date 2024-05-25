using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_Lab2.Data;
using WebAPI_Lab2.Dtos.Department;
using WebAPI_Lab2.Dtos.Student;
using WebAPI_Lab2.Models;

namespace WebAPI_Lab2.Repository
{
    public class HelperRepository : IHelperRepository
    {
        private readonly ItiContext _context;
        private readonly IMapper _mapper;

        public HelperRepository(ItiContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public bool Exists<T>(Func<T, bool> predicate) where T : class
        {
            return _context.Set<T>().Any(predicate);
        }

        public IEnumerable<StudentDto> SearchStudents(string keyword)
        {
           var list = _context.Students
                .Where(s => s.StFname.Contains(keyword) ||
                            s.StLname.Contains(keyword) ||
                            s.StAddress.Contains(keyword))
                .ToList();

            var searchlist = _mapper.Map<List<StudentDto>>(list);

            return searchlist;
        }

        public IEnumerable<DepartmentDto> SearchDepartments(string keyword)
        {
            var list = _context.Departments
                .Where(d => d.DeptName.Contains(keyword) ||
                            d.DeptDesc.Contains(keyword) ||
                            d.DeptLocation.Contains(keyword))
                .ToList();
            var searchlist = _mapper.Map<List<DepartmentDto>>(list);

            return searchlist;
        }
    }
}
