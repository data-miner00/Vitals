namespace Vitals.WebApi.Options;

public record class LogBackgroundServiceOption
{
    public const string SectionName = "LogBackgroundService";

    public bool Enabled { get; init; }

    public int IntervalInSeconds { get; init; }

    public int RandomLogMaxCount { get; init; }
}
