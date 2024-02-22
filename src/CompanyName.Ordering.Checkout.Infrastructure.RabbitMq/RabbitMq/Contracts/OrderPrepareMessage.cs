namespace Infrastructure.RabbitMq.Contracts;

public record OrderPrepareMessage
{

    public string? OrderId { get; set; }

    public DateTimeOffset OrderedAt { get; set; }

    public string? CustomerName { get; set; }

    public DateTimeOffset SentToKitchenAt { get; set; }
    
    public string? Message { get; set; }

};