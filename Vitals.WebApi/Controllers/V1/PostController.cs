namespace Vitals.WebApi.Controllers.V1;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vitals.Core.Models;
using Vitals.Core.Repositories;
using Vitals.WebApi.Models;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/User/{userId:int}/[controller]")]
[ApiController]
public sealed class PostController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public PostController(IUserRepository userRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePostRequest request)
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return this.Unauthorized("Invalid user ID.");
        }

        var user = await this.userRepository.GetByIdAsync(parsedUserId, this.CancellationToken);

        if (user is null)
        {
            return this.Unauthorized("Invalid user.");
        }

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await this.postRepository.AddAsync(post, this.CancellationToken);

        return this.Created();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int userId, int id)
    {
        var post = await this.postRepository.GetByIdAsync(id, this.CancellationToken);
        if (post is null)
        {
            return this.NotFound("Post not found.");
        }

        if (post.UserId != userId)
        {
            return this.Forbid("You do not have permission to access this post.");
        }

        return this.Ok(post);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var posts = await this.postRepository.GetAllAsync(this.CancellationToken);
        return this.Ok(posts);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int userId, int id, [FromBody] CreatePostRequest request)
    {
        var post = await this.postRepository.GetByIdAsync(id, this.CancellationToken);
        if (post is null)
        {
            return this.NotFound("Post not found.");
        }

        if (post.UserId != userId)
        {
            return this.Forbid("You do not have permission to update.");
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;
        await this.postRepository.UpdateAsync(post, this.CancellationToken);
        return this.NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int userId, int id)
    {
        try
        {
            var post = await this.postRepository.GetByIdAsync(id, this.CancellationToken);

            if (post?.UserId != userId)
            {
                return this.Forbid("You do not have permission to delete this post.");
            }

            await this.postRepository.RemoveAsync(id, this.CancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(ex.Message);
        }
    }
}
