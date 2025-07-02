namespace Vitals.Integrations.Clients;

using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Vitals.Core.Clients;

public sealed class RmqEventPublisher : IEventPublisher
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private readonly IChannel channel;
    private readonly string queueName;

    public RmqEventPublisher(IChannel channel, string queueName)
    {
        this.channel = channel;
        this.queueName = queueName;
    }

    public async Task PublishAsync(object @event, CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Serialize(@event, CachedJsonSerializerOptions);

        var body = Encoding.UTF8.GetBytes(message);

        await this.channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: this.queueName,
            body: body,
            cancellationToken: cancellationToken);
    }
}
