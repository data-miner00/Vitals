namespace Vitals.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Vitals.WebApi.Models;
using Vitals.Core.Models;
using Vitals.Core.Repositories;
using User = Core.Models.User;
using System.Security.Cryptography;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IRepository<User> userRepository;
    private readonly SHA512 hasher;

    public AuthenticationController(IRepository<User> userRepository, SHA512 hasher)
    {
        this.userRepository = userRepository;
        this.hasher = hasher;
    }

    private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var hashedPasswordBytes = this.hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        user.Credential = new Credential
        {
            PasswordHash = hashedPassword,
        };

        await this.userRepository.AddAsync(user, this.CancellationToken);

        return this.Created();
    }
}
