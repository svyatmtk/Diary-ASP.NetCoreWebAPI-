using Diary.Api;
using Diary.Api.Middlewares;
using Diary.Application.DependencyInjection;
using Diary.Consumer.DependencyInjection;
using Diary.DAL.DependencyInjection;
using Diary.Domain.Settings;
using Serilog;
using Diary.Producer.DependencyInjection;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddProducer();
builder.Services.AddConsumer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMetricServer();
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Diary Swagger v1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Diary Swagger v2.0");
        //c.RoutePrefix = string.Empty;
    });
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapGet("/random-number", () =>
{
    var number = Random.Shared.Next(1, 10);
    return Results.Ok(number);
});

app.MapMetrics();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();
