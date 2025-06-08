namespace Vitals.WebApi.Controllers.V1;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Vitals.WebApi.Models;
using Vitals.WebApi.Repositories;

[Area("users")]
[Authorize]
public sealed class UserController : V1Controller
{
    private readonly Counter<int> counter;
    private readonly ActivitySource source;
    private readonly ILogger<UserController> logger;
    private readonly UserRepository repository;

    public UserController(
        UserRepository repository,
        Meter greeterMeter,
        ActivitySource source,
        ILogger<UserController> logger)
    {
        this.repository = repository;
        this.counter = greeterMeter.CreateCounter<int>("Counter");
        this.source = source;
        this.logger = logger;
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        using var activity = this.source.StartActivity("GetById", ActivityKind.Server);
        try
        {
            this.counter.Add(1);
            this.logger.LogInformation("Fetching user with ID {Id}", id);
            return Ok(this.repository.Get(id));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error fetching user with ID {Id}", id);
            return BadRequest(ex.Message);
        }
        finally
        {
            activity?.SetTag("id", id);
            activity?.SetTag("controller", nameof(UserController));
            activity?.SetTag("action", nameof(GetById));
            activity?.SetTag("area", "users");
        }
    }

    [HttpGet]
    public IActionResult Get()
    {
        this.logger.LogInformation("Fetching all users");
        return Ok(this.repository.GetAll());
    }

    [HttpPost]
    public IActionResult Create([FromBody] User user)
    {
        try
        {
            this.repository.Add(user);
            this.logger.LogInformation("User created with ID {Id}", user.Id);
            return Created();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error creating user with Id {Id}", user.Id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        try
        {
            this.repository.Update(user);
            this.logger.LogInformation("User with ID {Id} has been updated", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error updating user with Id {Id}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("random")]
    public IActionResult FailRandomly()
    {
        var randomInt = new Random().Next(0, 2);

        using var _ = this.logger.BeginScope(new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("RandomInt", randomInt),
        });

        var fail = randomInt == 1;
        if (fail)
        {
            this.logger.LogError("Random failure was thrown!");
            throw new Exception("Random failure");
        }

        this.logger.LogInformation("Random failure not thrown");

        return Ok("Success");
    }

    [HttpGet("fail")]
    public IActionResult Fail()
    {
        throw new NotImplementedException();
    }
}
