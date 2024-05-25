using AutoMapper;
using WebAPI_Lab2.Dtos.Department;
using WebAPI_Lab2.Dtos.Student;
using WebAPI_Lab2.Models;

namespace WebAPI_Lab2.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.StFname != null ? src.StFname.Trim() : string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.StLname != null ? src.StLname.Trim() : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.StAddress != null ? src.StAddress.Trim() : string.Empty))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.StAge ?? 0))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Dept != null ? src.Dept.DeptName.Trim() : string.Empty))
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.StSuperNavigation != null ? string.Concat(src.StSuperNavigation.StFname, " ", src.StSuperNavigation.StLname).Trim() : string.Empty));

            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DeptName != null ? src.DeptName.Trim() : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DeptDesc != null ? src.DeptDesc.Trim() : string.Empty))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.DeptLocation != null ? src.DeptLocation.Trim() : string.Empty))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.DeptManagerNavigation != null ? src.DeptManagerNavigation.InsName : string.Empty))
                .ForMember(dest => dest.ManagerHiredate, opt => opt.MapFrom(src => src.ManagerHiredate ?? default))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students.Count()));
        }
    }
}
