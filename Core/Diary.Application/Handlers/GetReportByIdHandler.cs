using Diary.Application.Queries;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MediatR;

namespace Diary.Application.Handlers;

public class GetReportByIdHandler(IBaseRepository<Report> repository) : IRequestHandler<GetReportByIdQuery, ReportDto>
{
    public Task<ReportDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = repository.GetAll().AsEnumerable()
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
            .FirstOrDefault(x => x.Id == request.ReportId);

        return Task.FromResult(report);
    }
}