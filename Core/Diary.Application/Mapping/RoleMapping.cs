using AutoMapper;
using Diary.Domain.Dto.RoleDto;
using Diary.Domain.Entity;

namespace Diary.Application.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}