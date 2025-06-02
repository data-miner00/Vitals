namespace Vitals.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Vitals.WebApi.Models;
using Vitals.Core.Models;
using Vitals.Core.Repositories;
using User = Core.Models.User;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly SHA512 hasher;

    public AuthenticationController(IUserRepository userRepository, SHA512 hasher)
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var hashedPasswordBytes = this.hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        var user = await this.userRepository.GetByUsernameAsync(request.Username, this.CancellationToken);

        if (user is null || user.Credential is null || user.Credential.PasswordHash != hashedPassword)
        {
            return this.Unauthorized();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        return this.SignIn(principal, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
