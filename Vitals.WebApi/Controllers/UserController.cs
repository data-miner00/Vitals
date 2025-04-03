using Microsoft.AspNetCore.Mvc;

namespace Vitals.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository repository;

        public UserController(UserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(this.repository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
