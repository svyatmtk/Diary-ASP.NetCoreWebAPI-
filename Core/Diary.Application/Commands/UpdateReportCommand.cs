using Diary.Domain.Entity;
using MediatR;

namespace Diary.Application.Commands;

public record UpdateReportCommand(long Id, string Name, string Description, Report ReportToChange) : IRequest<Report>;