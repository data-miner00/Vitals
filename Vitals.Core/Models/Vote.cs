namespace Vitals.Core.Models;

public class Vote
{
    public int Id { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
