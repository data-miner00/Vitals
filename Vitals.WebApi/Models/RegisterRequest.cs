namespace Vitals.WebApi.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    public string Username { get; set; }

    [Required, MinLength(12)]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required, Phone]
    public string PhoneNumber { get; set; }
}
