namespace Vitals.Core.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Vote
{
    public int Id { get; set; }

    public int Count { get; set; }

    public int PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
