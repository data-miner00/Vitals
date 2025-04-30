using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Vitals.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Area("users")]
    public class UserController : ControllerBase
    {
        private readonly Counter<int> counter;
        private readonly ActivitySource source;
        private readonly UserRepository repository;

        public UserController(UserRepository repository, Meter greeterMeter, ActivitySource source)
        {
            this.repository = repository;
            this.counter = greeterMeter.CreateCounter<int>("Counter");
            this.source = source;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            using var activity = this.source.StartActivity("GetById", ActivityKind.Server);
            try
            {
                this.counter.Add(1);
                return Ok(this.repository.Get(id));
            }
            catch (Exception ex)
            {
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
            return Ok(this.repository.GetAll());
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            try
            {
                this.repository.Add(user);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            try
            {
                this.repository.Update(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
