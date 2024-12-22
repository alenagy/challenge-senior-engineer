using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

public class RabbitMqService : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public bool IsInitialized { get { return _connection != null && _channel != null; } }

    public RabbitMqService() { }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }
        
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = "guest",
            Password = "guest",
        };

        try
        {
            // Create a connection asynchronously
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: "syncQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }
        catch (BrokerUnreachableException ex)
        {
            Console.WriteLine($"RabbitMQ connection error: {ex.Message}");
            throw;
        }
    }

    public async Task PublishMessageAsync(string message)
    {
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync("", "syncQueue", body);
    }

    public async ValueTask DisposeAsync()
    {     
        if (_channel != null)
        {
            await _channel.CloseAsync();
        }
        if (_connection != null) 
        {
            await _connection.CloseAsync();
        }
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}