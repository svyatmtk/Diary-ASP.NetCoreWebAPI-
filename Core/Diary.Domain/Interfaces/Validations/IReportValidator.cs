using Diary.Domain.Entity;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Validations;

public interface IReportValidator : IBaseValidation<Report>
{
    /// <summary>
    /// Проверка наличия такого же отчёта по имени в БД, если отчёт с таким именем есть, то создать его нельзя
    /// Проверка наличия пользователя, если пользователь с таким Id не найдет, такого пользователя нет
    /// </summary>
    /// <param name="report"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    BaseResult CreateValidator(Report report, User user);
}