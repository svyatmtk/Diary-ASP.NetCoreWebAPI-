using Diary.Application.Queries;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.Handlers;

public class GetAllReportsHandler(IBaseRepository<Report> reportRepository)
    : IRequestHandler<GetAllReportsQuery, ReportDto[]>
{
    public async Task<ReportDto[]> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        return  await reportRepository.GetAll()
            .Where(x => x.UserId == request.UserId)
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
            .ToArrayAsync(cancellationToken: cancellationToken);
    }
}