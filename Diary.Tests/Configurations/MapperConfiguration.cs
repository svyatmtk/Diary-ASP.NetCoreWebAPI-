using AutoMapper;
using Diary.Application.Mapping;

namespace Diary.Tests.Configurations;

public static class MapperConfiguration
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(x =>
            x.AddProfile(new ReportMapping()));

        return mockMapper.CreateMapper();
    }
}