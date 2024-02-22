namespace Infrastructure.RabbitMq.Constants;

public static class QueueValues
{

    public static class Exchanges
    {

        public static string OrderingExchange => "ordering";

    }

    public static class RoutingKeys
    {

        public static string OrderRequestRoutingKey => "order-request";
        
        public static string OrderPrepareRoutingKey => "order-prepare";
        
        public static string OrderReadyRoutingKey => "order-ready";
        
        public static string OrderSentToCustomerRoutingKey => "order-sent-to-customer";

    }

    public static class Queues
    {

        public static string OrderRequestedQueueName => "order-requested";

        public static string OrderPrepareQueueName => "order-prepare";

        public static string OrderReadyQueueName => "order-ready";
        
        public static string OrderSentToCustomerQueueName => "order-sent-to-customer";
        
        public static string OrderLogsQueueName => "order-logs";

    }

}