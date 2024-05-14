using System.Text;
using Diary.Producer.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Diary.Producer;

public class Producer : IMessageProducer
{
    public  void SendMessage<T>(T message, string routingKey, string? exchange = default)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connections = factory.CreateConnection();
        using var channel = connections.CreateModel();

        var json = JsonConvert.SerializeObject(message, Formatting.Indented, 
            new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange, routingKey, body: body);
    }
}