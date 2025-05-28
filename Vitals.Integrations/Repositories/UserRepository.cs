namespace Vitals.Integrations.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vitals.Core;
using Vitals.Core.Models;
using Vitals.Core.Repositories;

public sealed class UserRepository : IRepository<User>
{
    private readonly AppDbContext context;

    public UserRepository(AppDbContext context)
    {
        this.context = Guard.ThrowIfNull(context);
    }

    public async Task AddAsync(User item, CancellationToken cancellationToken)
    {
        await this.context.Users.AddAsync(item, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await this.context.Users.ToListAsync(cancellationToken);
        return users;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");
        this.context.Users.Remove(user);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<User>> SearchAsync(string keyword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(User item, CancellationToken cancellationToken)
    {
        var targetUser = await this.context.Users.FirstAsync(x => x.Id == item.Id, cancellationToken);
        this.context.Users
            .Entry(targetUser)
            .CurrentValues
            .SetValues(item);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
