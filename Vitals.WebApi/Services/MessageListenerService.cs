namespace Vitals.WebApi.Services;

using System.Threading;
using System.Threading.Tasks;
using Vitals.Integrations.Clients;

public sealed class MessageListenerService : BackgroundService
{
    public MessageListenerService(RmqEventListener listener, ILogger<MessageListenerService> logger)
    {
        logger.LogInformation("Message Listening Initiated.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
