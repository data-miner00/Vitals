namespace Vitals.WebApi.Services;

using Vitals.Core;
using Vitals.WebApi.Options;

/// <summary>
/// The background service that logs messages at regular intervals.
/// </summary>
public sealed class LogBackgroundService : BackgroundService
{
    private const string ServiceName = nameof(LogBackgroundService);

    private readonly ILogger<LogBackgroundService> logger;
    private readonly LogBackgroundServiceOption option;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogBackgroundService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="option">Options for the service.</param>
    public LogBackgroundService(ILogger<LogBackgroundService> logger, LogBackgroundServiceOption option)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.option = Guard.ThrowIfNull(option);
    }

    /// <inheritdoc/>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return LogAndStop();

        async Task LogAndStop()
        {
            this.logger.LogInformation("Logging from {ServiceName} - StopAsync - {DateTime}", ServiceName, DateTime.Now);
            await base.StopAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        while (!stoppingToken.IsCancellationRequested)
        {
            var randomLogCount = Random.Shared.Next(1, this.option.RandomLogMaxCount);

            for (int i = 0; i < randomLogCount; i++)
            {
                this.logger.LogInformation(
                    "Logging from {ServiceName} - ExecuteAsync - {DateTime} - Log {LogNumber}",
                    ServiceName,
                    DateTime.Now,
                    i + 1);
            }

            await Task.Delay(TimeSpan.FromSeconds(this.option.IntervalInSeconds), stoppingToken);
        }
    }
}
