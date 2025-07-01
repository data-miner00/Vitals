namespace Vitals.WebApi.Controllers.V1;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vitals.Core.Models;
using Vitals.Core.Repositories;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/User/{userId:int}/Post/{postId:int}/[controller]")]
[ApiController]
public class VoteController : ControllerBase
{
    private readonly IVoteRepository voteRepository;
    private readonly IPostRepository postRepository;

    public VoteController(IVoteRepository voteRepository, IPostRepository postRepository)
    {
        this.voteRepository = voteRepository;
        this.postRepository = postRepository;
    }

    private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

    [HttpGet]
    public async Task<IActionResult> GetVote(int userId, int postId)
    {
        var vote = await this.voteRepository.GetByPostId(postId, this.CancellationToken);

        if (vote is null)
        {
            return this.NotFound("Vote not found.");
        }

        return this.Ok(vote);
    }

    [HttpPatch("upvote")]
    public async Task<IActionResult> IncrementAsync(int userId, int postId)
    {
        var post = await this.postRepository.GetByIdAsync(postId, this.CancellationToken);

        if (post is null)
        {
            return this.NotFound("Post not found.");
        }

        if (post.UserId != userId)
        {
            return this.BadRequest("The user ID specified does not match.");
        }

        var callerUserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        if (!int.TryParse(callerUserId, out var parsedUserId))
        {
            return this.Unauthorized("Invalid user ID.");
        }

        // TODO: Disallow voting on own posts

        var vote = await this.voteRepository.GetByPostId(postId, this.CancellationToken);

        if (vote is null)
        {
            var newVote = new Vote
            {
                PostId = postId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Count = 1,
            };

            await this.voteRepository.AddAsync(newVote, this.CancellationToken);
        }
        else
        {
            vote.Count++;
            vote.UpdatedAt = DateTime.UtcNow;
            await this.voteRepository.UpdateAsync(vote, this.CancellationToken);
        }

        return this.Ok();
    }

    [HttpPatch("downvote")]
    public async Task<IActionResult> DecrementAsync(int userId, int postId)
    {
        var post = await this.postRepository.GetByIdAsync(postId, this.CancellationToken);

        if (post is null)
        {
            return this.NotFound("Post not found.");
        }

        if (post.UserId != userId)
        {
            return this.BadRequest("The user ID specified does not match.");
        }

        var callerUserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        if (!int.TryParse(callerUserId, out var parsedUserId))
        {
            return this.Unauthorized("Invalid user ID.");
        }

        // TODO: Disallow voting on own posts

        var vote = await this.voteRepository.GetByPostId(postId, this.CancellationToken);

        if (vote is null)
        {
            var newVote = new Vote
            {
                PostId = postId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Count = 1,
            };

            await this.voteRepository.AddAsync(newVote, this.CancellationToken);
        }
        else
        {
            vote.Count--;
            vote.UpdatedAt = DateTime.UtcNow;
            await this.voteRepository.UpdateAsync(vote, this.CancellationToken);
        }

        return this.Ok();
    }

}
