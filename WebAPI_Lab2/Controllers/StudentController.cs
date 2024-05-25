using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebAPI_Lab2.Dtos.Student;
using WebAPI_Lab2.Models;
using WebAPI_Lab2.UnitOfWork;

namespace WebAPI_Lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "StudentEndpoints")]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork<Student> _unitOfWork;
        private readonly IMapper _mapper;

        public StudentController(IUnitOfWork<Student> context, IMapper mapper)
        {
            _unitOfWork = context;
            _mapper = mapper;

        }

        /// <summary>
        /// Retrieves all students with department and supervisor details.
        /// </summary>
        /// <returns>A list of students.</returns>
        [HttpGet("GetAllStudents")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _unitOfWork.Entity
                .GetAllIncluding(s => s.Dept, s => s.StSuperNavigation)
                .ToListAsync();

            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            return studentDtos;
        }

        /// <summary>
        /// Retrieves a single student by their ID.
        /// </summary>
        /// <param name="id">The ID of the student to retrieve.</param>
        /// <returns>A student DTO if found; otherwise, NotFound result.</returns>
        [HttpGet("GetStudentById/{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            var student = await _unitOfWork.Entity
                .GetAllIncluding(s => s.Dept, s => s.StSuperNavigation)
                .FirstOrDefaultAsync(s => s.StId == id);

            if (student == null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDto>(student);
            return studentDto;
        }

        /// <summary>
        /// Updates a student's details.
        /// </summary>
        /// <param name="id">The ID of the student to update.</param>
        /// <param name="student">The updated student data.</param>
        /// <returns>A NoContent result if successful; otherwise, appropriate error status.</returns>
        [HttpPut("EditStudent/{id}")]
        public async Task<IActionResult> EditStudent(int id, Student student)
        {
            if (id != student.StId)
                return BadRequest("Mismatched ID");

            _unitOfWork.Entity.UpdateAsync(student);

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.HelperRepository.Exists<Student>(s => s.StId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new student to the database.
        /// </summary>
        /// <param name="student">The student to add.</param>
        /// <returns>The created student with action to retrieve it by ID.</returns>
        [HttpPost("AddStudent")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Student>> AddStudent(Student student)
        {
            _unitOfWork.Entity.InsertAsync(student);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                if (_unitOfWork.HelperRepository.Exists<Student>(s => s.StId == student.StId))
                    return Conflict("A student with the same ID already exists.");

                throw;
            }

            return CreatedAtAction("GetStudentById", new { id = student.StId }, student);
        }

        /// <summary>
        /// Deletes a student by their ID.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <returns>A NoContent result if successful; otherwise, NotFound result.</returns>
        [HttpDelete("DeleteStudentById/{id}")]
        public async Task<IActionResult> DeleteStudentById(int id)
        {
            var student = await _unitOfWork.Entity.GetByIdAsync(id);

            if (student == null)
                return NotFound();

            _unitOfWork.Entity.DeleteAsync(student);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpGet("GetPagination")]
        public async Task<IActionResult> GetPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _unitOfWork.Entity
                    .GetAllIncluding(s => s.Dept, s => s.StSuperNavigation);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var results = await query.Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                var json = JsonSerializer.Serialize(new { TotalCount = totalCount, TotalPages = totalPages, Results = results }, jsonOptions);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
