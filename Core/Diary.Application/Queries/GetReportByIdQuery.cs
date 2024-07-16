using Diary.Domain.Dto.ReportDto;
using MediatR;

namespace Diary.Application.Queries;

public class GetReportByIdQuery(long reportId) : IRequest<ReportDto>
{
    public long ReportId { get; } = reportId;
}