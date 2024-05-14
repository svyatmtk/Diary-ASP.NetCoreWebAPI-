using AutoMapper;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Entity;

namespace Diary.Application.Mapping;

public class ReportMapping : Profile
{
    public ReportMapping()
    {
        CreateMap<Report, ReportDto>()
            .ForCtorParam(ctorParamName: "Id", expression => expression.MapFrom(s => s.Id))
            .ForCtorParam(ctorParamName: "Name", expression => expression.MapFrom(s => s.Name))
            .ForCtorParam(ctorParamName: "Description", expression => expression.MapFrom(s => s.Description))
            .ForCtorParam(ctorParamName: "DateCreated", expression => expression.MapFrom(s => s.CreatedAt))
            .ReverseMap();

    }
}