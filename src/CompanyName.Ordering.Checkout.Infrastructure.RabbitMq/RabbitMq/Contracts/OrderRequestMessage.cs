namespace Infrastructure.RabbitMq.Contracts;

public record OrderRequestMessage
{

    public string? OrderId { get; set; }

    public DateTimeOffset OrderedAt { get; set; }

    public string? CustomerName { get; set; }
    
    public string? Message { get; set; }

};