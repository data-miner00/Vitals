namespace Vitals.Integrations.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vitals.Core;
using Vitals.Core.Models;
using Vitals.Core.Repositories;

public sealed class PostRepository : IPostRepository
{
    private readonly AppDbContext context;

    public PostRepository(AppDbContext context)
    {
        this.context = Guard.ThrowIfNull(context);
    }

    public async Task AddAsync(Post item, CancellationToken cancellationToken)
    {
        await this.context.Posts.AddAsync(item, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken)
    {
        var posts = await this.context.Posts.ToListAsync(cancellationToken);
        return posts;
    }

    public Task<Post?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var post = this.context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return post;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var post = await this.context.Posts
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Post not found.");
        this.context.Posts.Remove(post);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Post>> SearchAsync(string keyword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Post item, CancellationToken cancellationToken)
    {
        var targetPost = await this.context.Posts.FirstAsync(x => x.Id == item.Id, cancellationToken);
        this.context.Posts
            .Entry(targetPost)
            .CurrentValues
            .SetValues(item);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
