namespace Vitals.Core.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Post
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    public int VoteId { get; set; }

    [ForeignKey(nameof(VoteId))]
    public Vote Vote { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
