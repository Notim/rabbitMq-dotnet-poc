using CompanyName.Ordering.Checkout.OrderReady.Worker.Consumers;

namespace CompanyName.Ordering.Checkout.OrderReady.Worker;

internal class Program
{

    public static void Main(string[] args)
    {
        // Add services to the container.
        var app = CreateHostBuilder(args).Build();

        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) => {
            // services.AddHostedService<OrderRequestedConsumer>();

            services.AddLogging();

            services.AddHostedService<OrderReadyConsumer>();
        });

}