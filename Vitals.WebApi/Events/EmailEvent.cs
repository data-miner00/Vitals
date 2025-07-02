namespace Vitals.WebApi.Events;

using System.ComponentModel.DataAnnotations;

public class EmailEvent
{
    [EmailAddress]
    public string Receiver { get; set; }

    [EmailAddress]
    public string Sender { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
}
