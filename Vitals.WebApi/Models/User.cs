namespace Vitals.WebApi.Models;

public sealed record class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
}
