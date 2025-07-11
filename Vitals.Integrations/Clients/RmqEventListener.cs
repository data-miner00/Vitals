namespace Vitals.Integrations.Clients;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Vitals.Core;

public sealed class RmqEventListener : IDisposable
{
    private readonly AsyncEventingBasicConsumer consumer;
    private readonly ILogger<RmqEventListener> logger;
    private bool isDisposed;

    public RmqEventListener(
        ILogger<RmqEventListener> logger,
        IChannel channel,
        string queueName)
    {
        Guard.ThrowIfNull(logger);
        Guard.ThrowIfNull(channel);
        Guard.ThrowIfNullOrWhitespace(queueName);

        this.consumer = new AsyncEventingBasicConsumer(channel);
        this.consumer.ReceivedAsync += this.StartHandlingAsync;

        channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: true,
            consumer: consumer)
            .GetAwaiter()
            .GetResult();

        this.logger = logger;
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.logger.LogInformation(
            "Disposing {ListenerName} at {DateTime}",
            nameof(RmqEventListener),
            DateTime.Now);

        this.consumer.ReceivedAsync -= this.StartHandlingAsync;
        this.isDisposed = true;
    }

    private Task StartHandlingAsync(object metadata, BasicDeliverEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Body.ToArray());

        this.logger.LogInformation(
            "Received message: '{Message}' from queue: {QueueName}",
            body,
            args.RoutingKey);

        return Task.CompletedTask;
    }
}
