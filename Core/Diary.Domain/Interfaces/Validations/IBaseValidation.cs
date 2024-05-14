using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Validations;

public interface IBaseValidation<in T> where T : class
{
    BaseResult ValidateOnNull(T model);
}