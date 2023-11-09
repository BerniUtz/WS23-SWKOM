using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic;

public class RabbitMQService: IQueueService
{
    private readonly IModel _channel;

    public RabbitMQService(string hostname, string username, string password)
    {
        var factory = new ConnectionFactory() {HostName = hostname, UserName = username, Password = password};
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }
    
    public Task CreateQueueAsync(string queueName)
    {
        _channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        return Task.CompletedTask;
    }

    public Task DeleteQueueAsync(string queueName)
    {
        _channel.QueueDelete(queueName);
        return Task.CompletedTask;
    }
    
    public Task EnqueueAsync<T>(string queueName, T messageObject)
    {
        var messageBody = JsonSerializer.Serialize(messageObject);
        var body = Encoding.UTF8.GetBytes(messageBody);
        _channel.BasicPublish(exchange:"", routingKey:queueName, basicProperties:null, body:body);
        return Task.CompletedTask;
    }

    public Task<T> DequeueAsync<T>(string queueName) where T : class
    {
        var result = _channel.BasicGet(queueName, true);
        if (result == null) return Task.FromResult<T>(default!);

        var body = result.Body.ToArray();
        var messageBody = Encoding.UTF8.GetString(body);
        var messageObject = JsonSerializer.Deserialize<T>(messageBody);
        
        if (messageObject == null) throw new Exception("Failed to deserialize message body.");
        
        return Task.FromResult(messageObject);
    }

    public Task<bool> QueueExistsAsync(string queueName)
    {
        try
        {
            _channel.QueueDeclarePassive(queueName);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            // If the queue does not exist, an exception will be thrown.
            return Task.FromResult(false);
        }
    }
}