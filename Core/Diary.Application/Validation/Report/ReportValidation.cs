using Diary.Application.Resources;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Result;

namespace Diary.Application.Validation.Report;

public class ReportValidation : IReportValidator
{
    public BaseResult ValidateOnNull(Domain.Entity.Report? model)
    {
        if (model == null)
        {
            return new BaseResult()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int?)ErrorCodes.ReportNotFound
            };
        }

        return new BaseResult();
    }

    public BaseResult CreateValidator(Domain.Entity.Report? report, User? user)
    {
        if (report != null)
        {
            return new BaseResult()
            {
                ErrorMessage = ErrorMessage.ReportIsAlreadyExists,
                ErrorCode = (int?)ErrorCodes.ReportIsAlreadyExists
            };
        }

        if (user == null)
        {
            return new BaseResult()
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int?)ErrorCodes.UserNotFound
            };
        }

        return new BaseResult();
    }
}