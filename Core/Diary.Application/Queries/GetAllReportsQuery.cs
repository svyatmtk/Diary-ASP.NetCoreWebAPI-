using Diary.Domain.Dto.ReportDto;
using MediatR;

namespace Diary.Application.Queries;

public class GetAllReportsQuery(long userId) : IRequest<ReportDto[]>
{
    public long UserId { get; } = userId;
}