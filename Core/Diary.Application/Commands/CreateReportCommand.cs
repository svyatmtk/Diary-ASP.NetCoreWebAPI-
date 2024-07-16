using Diary.Domain.Entity;
using MediatR;

namespace Diary.Application.Commands;

public record CreateReportCommand(string Name, string Description, long UserId) : IRequest<Report>;