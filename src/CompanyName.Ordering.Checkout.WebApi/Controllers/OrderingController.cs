using System.Text;
using System.Text.Json;
using Bogus;
using Infrastructure.RabbitMq.Constants;
using Infrastructure.RabbitMq.Contracts;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace CompanyName.Ordering.Checkout.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderingController : ControllerBase
{
    private readonly ILogger<OrderingController> _logger;
    private readonly IConfiguration configuration;

    public OrderingController(
        ILogger<OrderingController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        this.configuration = configuration;
    }

    [HttpPost(Name = "OrderRequest")]
    public IActionResult Post([FromBody] OrderRequestMessage request)
    {
        
        try {
            var factory = new ConnectionFactory(){
                Uri = new Uri(configuration["RabbitMq:ConnectionString"]!)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
                request.Message = "pedido solicitado com sucesso";
                
                var jsonMessage = JsonSerializer.Serialize(request);
                
                channel.BasicPublish(
                    exchange: QueueValues.Exchanges.OrderingExchange, 
                    routingKey: QueueValues.RoutingKeys.OrderRequestRoutingKey, 
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(jsonMessage)
                );

                _logger.LogInformation("Concluido o envio de mensagem {index}", jsonMessage);
        } catch (Exception ex) {
            _logger.LogError($"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
        }    
        
        return Accepted();
    }

}