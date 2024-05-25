namespace WebAPI_Lab2.Dtos.Department
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? Manager { get; set; }

        public DateOnly? ManagerHiredate { get; set; }
        public int? StudentCount { get; set; }
    }
}
