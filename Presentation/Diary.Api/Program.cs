using Diary.Api;
using Diary.Api.Middlewares;
using Diary.Application.DependencyInjection;
using Diary.Consumer.DependencyInjection;
using Diary.DAL.DependencyInjection;
using Diary.Domain.Settings;
using Serilog;
using Diary.Producer.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddControllers();
builder.Services.AddSwagger();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddProducer();
builder.Services.AddConsumer();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

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

app.MapControllers();
app.UseHttpsRedirection();
app.Run();
