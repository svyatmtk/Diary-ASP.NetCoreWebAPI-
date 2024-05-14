# **DiaryProject**
Проект представляет собой Web Api для дневника, который позволяет пользователям заполнять отчёта и сохранять, редактировать и удалять их.

# Технологический стек проекта:
* ASP.Net core
*  Entity Framework core
*  AutoMapper
*  FluentValidation
*  JWT Bearer для аутентификации
*  Serilog для логгирования
*  RabbitMQ - брокер сообщений
*  Fluent Validation

# Проект разделён на три слоя:
* Presentation - WebApi (REST API)
* Infrastructure - Работа с DBContext, брокерами сообщений
* Core - ядро проекта (Application для сервисов, выступающих посредниками между контроллерами и базами данных, Domain для связанных с проектом Entity и DTO).
* В проекте также используется Dependency Injection для регистрации всех зависимостей для конфигурации Web Api в одном месте.

# Разделы проекта
* Diary.Domain - содержит описание сущностей, DTO, Интерфейсов а также некоторых вспомогательных классов.

## Сущности
``` C#
public class User : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Report> Reports { get; set; }
        public List<Role> Roles { get; set; }
        public UserToken UserToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
    }
```
* Сущности должны реализовывать интерфейс IEntityId для добавления свойства Id в качестве первичного ключа, также есть необязательный интерфейс IAuditable, добавляющий информацию о дате создания и изменения записи в БД.

## DTO
``` C#
public record UserDto(string Login);
```
* DTO представляют из себя Record тип, содержащий необходимые данные для работы с контроллерами.

## BaseResult

* BaseResult - класс, использующийся для возвращения данных после выполнения транзакции. Он содержит информацию об успешности выполнения, возможных ошибках и также содержит свойство Data, в которое сохраняется результат работы транзакции.
``` c#
public class BaseResult
{
    public bool IsSuccess => ErrorMessage == null;
    public string ErrorMessage { get; set; }
    public int? ErrorCode { get; set; }
}

public class BaseResult<T> : BaseResult
{
    public BaseResult(string errorMessage, int errorCode, T data)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Data = data;
    }

    public BaseResult() {}
    public T Data { get; set; }
}
```
* Также имеется версия CollectionResult<T> для возвращения коллекции результатов

``` c#
public class CollectionResult<T> : BaseResult<IEnumerable<T>>
{
    public int Count { get; set; }
}
```
## Diary.Application 
Содержит в себе реализацию сервисов, Fluent Validation, AutoMapper, также имеет класс Dependency Injection.

* Реализация сервисов
  Сервис должен реализовывать интерфейс IReportService. Интерфейсы являются middleware для обращения к БД и возвращению ответа в контроллеры.
  ``` c#
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
    ```
  * Fluent Validation
    Содержит в себе правила для проверки DTO на соответствие ограничениям для полей в записи БД.
    
``` c#
public class CreateReportValidation : AbstractValidator<CreateReportDto>
{
    public CreateReportValidation()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
```

* AutoMapper
Служит для маппинга из сущностей в DTO или наоборот.
``` c#
public class ReportMapping : Profile
{
    public ReportMapping()
    {
        CreateMap<Report, ReportDto>()
            .ForCtorParam(ctorParamName: "Id", expression => expression.MapFrom(s => s.Id))
            .ForCtorParam(ctorParamName: "Name", expression => expression.MapFrom(s => s.Name))
            .ForCtorParam(ctorParamName: "Description", expression => expression.MapFrom(s => s.Description))
            .ForCtorParam(ctorParamName: "DateCreated", expression => expression.MapFrom(s => s.CreatedAt))
            .ReverseMap();

    }
}
```

* Dependency Injection
  Расширяет поведение типов, реализующих интерфейс IServiceCollection

  ``` c#
   public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ReportMapping));
        
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
  ```
# Diary.DAL
Слой, отвечающий за работу с базой данных

## Configurations 
Конфигурация сущностей при помощи Fluent Api
``` c#
public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Login).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Password).IsRequired();

            builder.HasMany<Report>(x => x.Reports)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .HasPrincipalKey(x => x.Id);

            builder.HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<UserRole>(x => x.HasOne<Role>().WithMany().HasForeignKey(role => role.RoleId),
                    x => x.HasOne<User>().WithMany().HasForeignKey(role => role.UserId));
            builder.HasData(new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Login = "svyatmtk",
                    Password = "password",
                    CreatedAt = DateTime.UtcNow,
                }
            });
        }
    }
```
##Interceptors
Перехватчики изменений в ChangeTracker для автоматического заполнения некоторых полей в записи.

``` c#
public class DateInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            
            var dbContext = eventData.Context;

            if (dbContext == null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = dbContext.ChangeTracker.Entries<IAuditable>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(X => X.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
```

