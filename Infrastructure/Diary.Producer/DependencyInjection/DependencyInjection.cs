using Diary.Producer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.Producer.DependencyInjection;

public static class DependencyInjection
{
    public static void AddProducer(this IServiceCollection services)
    {
        services.AddScoped<IMessageProducer, Producer>();
    }
}