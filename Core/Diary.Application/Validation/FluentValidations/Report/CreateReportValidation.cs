using Diary.Domain.Dto.ReportDto;
using FluentValidation;

namespace Diary.Application.Validation.FluentValidations.Report;

public class CreateReportValidation : AbstractValidator<CreateReportDto>
{
    public CreateReportValidation()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}