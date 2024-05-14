using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace Diary.Application.Services;

public class ReportService : IReportService
{
    private readonly IMessageProducer _messageProducer;
    private readonly IOptions<RabbitMqSettings> _options;
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IReportValidator _reportValidator;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public ReportService(IBaseRepository<Report> reportRepository, ILogger serilog,
        IBaseRepository<User> userRepository, IReportValidator reportValidator, IMapper mapper,
        IMessageProducer messageProducer, IOptions<RabbitMqSettings> options)
    {
        _reportRepository = reportRepository;
        _logger = serilog;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _options = options;
    }

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
    {
        ReportDto[] reports;

        reports = await _reportRepository.GetAll()
            .Where(x => x.UserId == userId)
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
            .ToArrayAsync();

        if (!reports.Any())
        {
            _logger.Warning(ErrorMessage.ReportsNotFound);
            return new CollectionResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportsNotFound, ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        return new CollectionResult<ReportDto>() { Data = reports, Count = reports.Length };
    }

    /// <inheritdoc />
    public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;

        try
        {
            report = _reportRepository.GetAll().AsEnumerable()
                .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                .FirstOrDefault(x => x.Id == id);
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return Task.FromResult(new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            });
        }

        if (report == null)
        {
            _logger.Warning($"Отчёт с {id} не найден");
            return Task.FromResult(new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            });
        }

        return Task.FromResult(new BaseResult<ReportDto>()
        {
            Data = report
        });
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReport(CreateReportDto dto)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
        var result = _reportValidator.CreateValidator(report, user);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>() { ErrorMessage = result.ErrorMessage, ErrorCode = result.ErrorCode };
        }

        report = new Report() { Name = dto.Name, Description = dto.Description, UserId = user.Id };
        await _reportRepository.CreateAsync(report);
        await _reportRepository.SaveChangesAsync();

        _messageProducer.SendMessage(report, _options.Value.RoutingKey, _options.Value.ExchangeName);

        return new BaseResult<ReportDto>() { Data = _mapper.Map<ReportDto>(report) };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
    {
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        var result = _reportValidator.ValidateOnNull(report);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>() { ErrorMessage = result.ErrorMessage, ErrorCode = result.ErrorCode };
        }

        _reportRepository.Remove(report);
        _reportRepository.SaveChangesAsync();

        return new BaseResult<ReportDto>() { Data = _mapper.Map<ReportDto>(report) };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto reportDto)
    {
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == reportDto.Id);
        var result = _reportValidator.ValidateOnNull(report);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        report.Name = reportDto.Name;
        report.Description = reportDto.Description;

        var updatedReport = _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync();

        return new BaseResult<ReportDto>()
        {
            Data = _mapper.Map<ReportDto>(updatedReport)
        };
    }
}