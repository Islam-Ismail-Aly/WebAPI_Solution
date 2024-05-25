namespace WebAPI_Lab2.Dtos.Student
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public string? DepartmentName { get; set; }
        public string? Supervisor { get; set; }
    }
}
