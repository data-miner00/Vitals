namespace Vitals.Integrations.Clients;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

public sealed class RmqEventListener : IDisposable
{
    private readonly AsyncEventingBasicConsumer consumer;

    private bool isDisposed;

    public RmqEventListener(IChannel channel, string queueName)
    {
        this.consumer = new AsyncEventingBasicConsumer(channel);
        this.consumer.ReceivedAsync += this.StartHandlingAsync;

        channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: true,
            consumer: consumer)
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.consumer.ReceivedAsync -= this.StartHandlingAsync;
        this.isDisposed = true;
    }

    private async Task StartHandlingAsync(object metadata, BasicDeliverEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Body.ToArray());

        Console.WriteLine(body);

        await Task.CompletedTask;
    }
}
