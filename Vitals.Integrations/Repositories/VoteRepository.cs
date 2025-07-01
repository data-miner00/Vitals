namespace Vitals.Integrations.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vitals.Core;
using Vitals.Core.Models;
using Vitals.Core.Repositories;

public sealed class VoteRepository : IVoteRepository
{
    private readonly AppDbContext context;

    public VoteRepository(AppDbContext context)
    {
        this.context = Guard.ThrowIfNull(context);
    }

    public async Task AddAsync(Vote item, CancellationToken cancellationToken)
    {
        await this.context.Votes.AddAsync(item, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Vote>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Vote?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var vote = await this.context.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return vote;
    }

    public async Task<Vote?> GetByPostId(int postId, CancellationToken cancellationToken)
    {
        var vote = await this.context.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PostId == postId, cancellationToken);

        return vote;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var vote = await this.context.Votes
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Vote not found.");
        this.context.Votes.Remove(vote);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Vote>> SearchAsync(string keyword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Vote item, CancellationToken cancellationToken)
    {
        var targetVote = await this.context.Votes.FirstAsync(x => x.Id == item.Id, cancellationToken);
        this.context.Votes
            .Entry(targetVote)
            .CurrentValues
            .SetValues(item);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
