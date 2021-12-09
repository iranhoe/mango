﻿namespace Mango.Services.PaymentAPI.Extensions;

using OrderAPI.Messaging;

public static class ApplicationBuilderExtensions
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopped.Register(OnStop);

        return app;
    }

    private static void OnStop()
    {
        ServiceBusConsumer.Stop();
    }

    private static void OnStart()
    {
        ServiceBusConsumer.Start();
    }
}