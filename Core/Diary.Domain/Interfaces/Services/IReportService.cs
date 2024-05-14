using Diary.Domain.Dto;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

/// <summary>
/// Сервис, отвечающий за работу с доменной частью отчёта (Report)
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Получение всех отчётов пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);
    
    /// <summary>
    /// Получение отчёта по Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> GetReportByIdAsync(long id);

    /// <summary>
    /// Создание нового отчёта
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> CreateReport(CreateReportDto dto);
    
    /// <summary>
    /// Удаление отчёта по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id);
    
    /// <summary>
    /// Обновить отчёт (имя или описание)
    /// </summary>
    /// <param name="reportDto"></param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto reportDto);
}