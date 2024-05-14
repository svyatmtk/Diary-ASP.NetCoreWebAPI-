using AutoMapper;
using Diary.Domain.Dto.UserDto;
using Diary.Domain.Entity;

namespace Diary.Application.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}