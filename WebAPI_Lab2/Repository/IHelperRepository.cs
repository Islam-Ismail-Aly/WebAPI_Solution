using Microsoft.AspNetCore.Mvc;
using WebAPI_Lab2.Dtos.Department;
using WebAPI_Lab2.Dtos.Student;
using WebAPI_Lab2.Models;

namespace WebAPI_Lab2.Repository
{
    public interface IHelperRepository
    {
        public bool Exists<T>(Func<T, bool> predicate) where T : class;
        public IEnumerable<DepartmentDto> SearchDepartments(string keyword);
        public IEnumerable<StudentDto> SearchStudents(string keyword);
    }
}
