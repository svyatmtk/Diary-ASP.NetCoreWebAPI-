using Diary.Application.Commands;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MediatR;

namespace Diary.Application.Handlers;

internal class UpdateReportHandler(IBaseRepository<Report> repository) : IRequestHandler<UpdateReportCommand, Report>
{
    public async Task<Report> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        request.ReportToChange.Name = request.Name;
        request.ReportToChange.Description = request.Description;

        var updatedReport = repository.Update(request.ReportToChange);
        await repository.SaveChangesAsync();

        return updatedReport;
    }
}