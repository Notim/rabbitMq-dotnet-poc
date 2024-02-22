using System.Text;
using System.Text.Json;
using Infrastructure.RabbitMq.Constants;
using Infrastructure.RabbitMq.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CompanyName.Ordering.Checkout.OrderRequested.Worker.Consumers;

public class OrderRequestedConsumer : BackgroundService
{

    private readonly ILogger<OrderRequestedConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly IConfiguration _configuration;

    public OrderRequestedConsumer(ILogger<OrderRequestedConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var factory = new ConnectionFactory(){
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]!)
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

        _channel.BasicConsume(queue: QueueValues.Queues.OrderRequestedQueueName, autoAck: false, consumer);
        
        
        return Task.CompletedTask;
    }

    private void HandleMessage(string content)
    {

        var desserialized = JsonSerializer.Deserialize<OrderRequestMessage>(content);
        
        _logger.LogInformation($"consumer received {desserialized.OrderId} | {desserialized.OrderedAt} | {desserialized.CustomerName} | {desserialized.Message}");
        
        // Thread.Sleep(800);
        
        try {
            var factory = new ConnectionFactory(){
                Uri = new Uri(_configuration["RabbitMq:ConnectionString"]!)
            };

            using var connection = factory.CreateConnection();
            using var channel    = connection.CreateModel();
            
            channel.BasicPublish(
                exchange: QueueValues.Exchanges.OrderingExchange, 
                routingKey: QueueValues.RoutingKeys.OrderPrepareRoutingKey, 
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new OrderPrepareMessage{
                    OrderId = desserialized.OrderId,
                    CustomerName = desserialized.CustomerName,
                    OrderedAt = desserialized.OrderedAt,
                    SentToKitchenAt = DateTimeOffset.Now,
                    Message = "Pedido enviado para a cozinha"
                }))
            );

            _logger.LogInformation("order preparation comand sent to exchange");
            
        } catch (Exception ex) {
            
            _logger.LogError($"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
        }

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