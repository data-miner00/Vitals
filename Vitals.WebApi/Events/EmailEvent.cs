namespace Vitals.WebApi.Events;

using System.ComponentModel.DataAnnotations;

public class EmailEvent
{
    [EmailAddress]
    public string Receiver { get; set; }

    [EmailAddress]
    public string Sender { get; set; }

    [RegularExpression(@"^[A-Z]_[0-9]$")]
    public string Subject { get; set; }

    public string Body { get; set; }
}
