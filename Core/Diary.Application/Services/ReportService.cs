using AutoMapper;
using Diary.Application.Commands;
using Diary.Application.Queries;
using Diary.Application.Resources;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Extensions;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Diary.Producer.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public ReportService(IBaseRepository<Report> reportRepository, ILogger serilog,
        IBaseRepository<User> userRepository, IReportValidator reportValidator, IMapper mapper,
        IMessageProducer messageProducer, IOptions<RabbitMqSettings> options, IDistributedCache distributedCache, IMediator mediator)
    {
        _reportRepository = reportRepository;
        _logger = serilog;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _options = options;
        _distributedCache = distributedCache;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
    {
        var reports = await _mediator.Send(new GetAllReportsQuery(userId));

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
    public async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;

        try
        {
            report = await _mediator.Send(new GetReportByIdQuery(id));
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return new BaseResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }

        if (report.Name == null)
        {
            _logger.Warning($"Отчёт с {id} не найден");
            
            return new BaseResult<ReportDto>
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            };
        }
        
        _distributedCache.SetObject($"Report_{id}", report);
        
        return new BaseResult<ReportDto>()
        {
            Data = report
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
        var result = _reportValidator.CreateValidator(report, user);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>() { ErrorMessage = result.ErrorMessage, ErrorCode = result.ErrorCode };
        }

        var newReport = await _mediator.Send(new CreateReportCommand(dto.Name, dto.Description, dto.UserId));

        _messageProducer.SendMessage(newReport, _options.Value.RoutingKey, _options.Value.ExchangeName);

        return new BaseResult<ReportDto>() { Data = _mapper.Map<ReportDto>(newReport) };
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
        
        var updatedReport = _mediator.Send(new UpdateReportCommand(reportDto.Id, reportDto.Name, reportDto.Description, report));
        
        return new BaseResult<ReportDto>()
        {
            Data = _mapper.Map<ReportDto>(updatedReport)
        };
        
    }
}