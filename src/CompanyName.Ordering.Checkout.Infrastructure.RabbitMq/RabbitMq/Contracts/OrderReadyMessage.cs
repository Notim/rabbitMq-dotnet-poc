namespace Infrastructure.RabbitMq.Contracts;

public record OrderReadyMessage
{

    public string? OrderId { get; set; }

    public DateTimeOffset OrderedAt { get; set; }

    public string? CustomerName { get; set; }

    public DateTimeOffset ReadyAt { get; set; }

    public string? Message { get; set; }

};