namespace Vitals.WebApi.Options;

public class MessagingOption
{
    public const string SectionName = "Messaging";

    public string HostName { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}
