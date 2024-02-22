using System.Text;
using Infrastructure.RabbitMq.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CompanyName.Ordering.Checkout.OrderSentToCustomer.Worker.Consumers;

public class OrderReadyConsumer : BackgroundService
{

    private readonly ILogger<OrderReadyConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public OrderReadyConsumer(ILogger<OrderReadyConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory(){
            Uri = new Uri(configuration["RabbitMq:ConnectionString"]!)
        };

        // create connection  
        _connection = factory.CreateConnection();

        // create channel  
        _channel = _connection.CreateModel();

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) => {
            // received message  
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            // handle the received message  
            HandleMessage(content);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume(queue: QueueValues.Queues.OrderSentToCustomerQueueName, autoAck: false, consumer);
        
        return Task.CompletedTask;
    }

    private void HandleMessage(string content)
    {
        // we just print this message
        _logger.LogInformation($"consumer received {content}");
    }

    private void OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e)
    { }

    private void OnConsumerUnregistered(object? sender, ConsumerEventArgs e)
    { }

    private void OnConsumerRegistered(object? sender, ConsumerEventArgs e)
    { }

    private void OnConsumerShutdown(object? sender, ShutdownEventArgs e)
    { }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    { }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }

}