namespace Mango.Services.OrderAPI.Extensions;

using Messages;

public static class ApplicationBuilderExtensions
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStarted.Register(OnStop);

        return app;
    }

    private static void OnStop()
    {
        ServiceBusConsumer.Start();
    }

    private static void OnStart()
    {
        ServiceBusConsumer.Stop();
    }
}