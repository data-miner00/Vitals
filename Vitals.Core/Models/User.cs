namespace Vitals.Core.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// Automatically sets by the database with autoincrement.
    /// </summary>
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public int CredentialId { get; set; }

    [ForeignKey(nameof(CredentialId))]
    public Credential Credential { get; set; }

    public List<Post> Posts { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
