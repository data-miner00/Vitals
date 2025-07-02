namespace Vitals.WebApi.Options;

public class EmailOption
{
    public const string SectionName = "Email";

    public string QueueName { get; set; }

    public string Sender { get; set; }
}
