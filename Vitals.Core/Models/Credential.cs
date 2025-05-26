namespace Vitals.Core.Models;

using System;

public class Credential
{
    public int Id { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
