using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI_Lab2.Dtos.Department;
using WebAPI_Lab2.Models;
using WebAPI_Lab2.UnitOfWork;

namespace WebAPI_Lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "DepartmentEndpoints")]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork<Department> _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentController(IUnitOfWork<Department> context, IMapper mapper)
        {
            _unitOfWork = context;
            _mapper = mapper;
        }

        [HttpGet("GetAllDepartments")]
        [SwaggerOperation(Summary = "Retrieves all departments, including details about their managers.")]
        [SwaggerResponse(200, "A list of departments with detailed information.", typeof(IEnumerable<DepartmentDto>))]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
        {
            var departments = await _unitOfWork.Entity
                .GetAllIncluding(s => s.Students, d => d.DeptManagerNavigation)
                .ToListAsync();

            var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);
            return departmentDtos;
        }

        [HttpGet("GetDepartmentById/{id}")]
        [SwaggerOperation(Summary = "Retrieves a specific department by ID, including manager details.")]
        [SwaggerResponse(200, "The department details if found.", typeof(DepartmentDto))]
        [SwaggerResponse(404, "Department not found.")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            var department = await _unitOfWork.Entity
                .GetAllIncluding(s => s.Students, d => d.DeptManagerNavigation)
                .FirstOrDefaultAsync(d => d.DeptId == id);

            if (department == null)
                return NotFound();

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return departmentDto;
        }

        [HttpPut("EditDepartment/{id}")]
        [SwaggerOperation(Summary = "Updates an existing department's details.")]
        [SwaggerResponse(204, "Department updated successfully.")]
        [SwaggerResponse(400, "The ID provided does not match the department's ID.")]
        [SwaggerResponse(404, "Department not found.")]
        public async Task<IActionResult> EditDepartment(int id, Department department)
        {
            if (id != department.DeptId)
                return BadRequest("Mismatched ID");

            _unitOfWork.Entity.UpdateAsync(department);

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.HelperRepository.Exists<Department>(d => d.DeptId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpPost("AddDepartment")]
        [SwaggerOperation(Summary = "Adds a new department to the database.")]
        [SwaggerResponse(201, "Department added successfully.", typeof(Department))]
        [SwaggerResponse(409, "Conflict occurred due to existing department ID.")]
        public async Task<ActionResult<Department>> AddDepartment(Department department)
        {
            _unitOfWork.Entity.InsertAsync(department);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                if (_unitOfWork.HelperRepository.Exists<Department>(d => d.DeptId == department.DeptId))
                    return Conflict("A department with the same ID already exists.");

                throw;
            }

            return CreatedAtAction("GetDepartmentById", new { id = department.DeptId }, department);
        }

        [HttpDelete("DeleteDepartmentById/{id}")]
        [SwaggerOperation(Summary = "Deletes a department by ID.")]
        [SwaggerResponse(204, "Department deleted successfully.")]
        [SwaggerResponse(404, "Department not found.")]
        public async Task<IActionResult> DeleteDepartmentById(int id)
        {
            var department = await _unitOfWork.Entity.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            _unitOfWork.Entity.DeleteAsync(department);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

    }
}
