using Diary.Application.Mapping;
using Diary.Application.Services;
using Diary.Application.Validation.FluentValidations.Report;
using Diary.Application.Validation.Report;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ReportMapping));

        var options = configuration.GetSection(nameof(RedisSettings));
        var redisUrl = options["Url"];
        var instanceName = options["InstanceName"];

        services.AddStackExchangeRedisCache(cacheOptions =>
        {
            cacheOptions.Configuration = redisUrl;
            cacheOptions.InstanceName = instanceName;
        });
        
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IReportValidator, ReportValidation>();
        
        services.AddScoped<IValidator<CreateReportDto>, CreateReportValidation>();
        services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidation>();
        
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}