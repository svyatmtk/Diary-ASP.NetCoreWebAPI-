using Diary.Application.Commands;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MediatR;

namespace Diary.Application.Handlers;

public class CreateReportHandler(IBaseRepository<Report> _repository) : IRequestHandler<CreateReportCommand, Report>
{
    
    public async Task<Report> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report()
        {
            Name = request.Name, 
            Description = request.Description, 
            UserId = request.UserId
        };
        await _repository.CreateAsync(report);
        await _repository.SaveChangesAsync();

        return report;

    }
}