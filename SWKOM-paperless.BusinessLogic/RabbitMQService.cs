using System.Text;
using System.Text.Json;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic
{
    public class RabbitMQService : IQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConnectionFactory _factory;

        public RabbitMQService(string hostname, string username, string password, int port = 5672)
        {
            _factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password, Port = port };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        
        public RabbitMQService(IConnection connection, IModel channel, ConnectionFactory factory)
        {
            _connection = connection;
            _channel = channel;
            _factory = factory;
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

        public async Task EnsureQueueExistsAsync(string queueName)
        {
            if (!await QueueExistsAsync(queueName))
            {
                await CreateQueueAsync(queueName);
            }
        }

        public Task EnqueueAsync<T>(string queueName, T messageObject)
        {
            var messageBody = JsonSerializer.Serialize(messageObject);
            var body = Encoding.UTF8.GetBytes(messageBody);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
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
            using var tempConnection = _factory.CreateConnection();
            using var tempChannel = tempConnection.CreateModel();
            try
            {
                tempChannel.QueueDeclarePassive(queueName);
                return Task.FromResult(true);
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex) when (ex.ShutdownReason.ReplyCode == 404)
            {
                // If the queue does not exist, a 404 code exception will be thrown.
                return Task.FromResult(false);
            }
        }

        public void Subscribe<T>(string queueName, Action<T> messageHandler) where T: class
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, args) => 
            {
                var body = args.Body.ToArray();
                var messageBody = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(messageBody);

                if (message == null)
                {
                    //empty message can be ignored
                }
                else
                {
                    messageHandler(message);
                }
                
            };
            
            // Start consuming messages from the queue
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}


public class RabbitMQOptions
{
    public required string Hostname { get; set; }
    public int Port { get; set; } = 5672;
    public required string Username { get; set; }
    public required string Password { get; set; }
}