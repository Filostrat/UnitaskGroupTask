using Application.DTOs;

using AutoMapper;

using Domain.Models;


namespace Application.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDto>().ForMember(d => d.Children, o => o.Ignore());

        CreateMap<Employee, EmployeeDto>().ForMember(d => d.Children, o => o.Ignore());
    }
}