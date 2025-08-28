namespace Vitals.WebApi.Controllers.V1;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitals.Core;
using Vitals.Core.Models;
using Vitals.Core.Repositories;
using Vitals.WebApi.Attributes;
using Vitals.WebApi.Models;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/")]
[ApiController]
public sealed class PostController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public PostController(IUserRepository userRepository, IPostRepository postRepository)
    {
        this.userRepository = Guard.ThrowIfNull(userRepository);
        this.postRepository = Guard.ThrowIfNull(postRepository);
    }

    private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

    [HttpGet("[controller]")]
    public async Task<IActionResult> GetAllAsync()
    {
        var posts = await this.postRepository.GetAllAsync(this.CancellationToken);
        return this.Ok(posts);
    }

    [UserAuthorize]
    [HttpPost("User/{userId:int}/[controller]")]
    public async Task<IActionResult> CreateAsync(int userId, [FromBody] CreatePostRequest request)
    {
        var user = await this.userRepository.GetByIdAsync(userId, this.CancellationToken);

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

    [UserAuthorize]
    [HttpGet("User/{userId:int}/[controller]/{id:int}")]
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

    [UserAuthorize]
    [HttpGet("User/{userId:int}/[controller]")]
    public async Task<IActionResult> GetAllByUserAsync(int userId)
    {
        var posts = await this.postRepository.GetAllAsync(this.CancellationToken);
        return this.Ok(posts.Where(x => x.UserId == userId).ToList());
    }

    [UserAuthorize]
    [HttpPut("User/{userId:int}/[controller]/{id:int}")]
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

    [UserAuthorize]
    [HttpDelete("User/{userId:int}/[controller]/{id:int}")]
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
