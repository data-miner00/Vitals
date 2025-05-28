namespace Vitals.Integrations.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vitals.Core;
using Vitals.Core.Models;
using Vitals.Core.Repositories;

public sealed class CredentialRepository : IRepository<Credential>
{
    private readonly AppDbContext context;

    public CredentialRepository(AppDbContext context)
    {
        this.context = Guard.ThrowIfNull(context);
    }

    public async Task AddAsync(Credential item, CancellationToken cancellationToken)
    {
        await this.context.Credentials.AddAsync(item, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Credential>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Credential?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cred = await this.context.Credentials
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return cred;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var cred = await this.context.Credentials
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Credential not found.");
        this.context.Credentials.Remove(cred);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Credential>> SearchAsync(string keyword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Credential item, CancellationToken cancellationToken)
    {
        var targetCredential = await this.context.Credentials.FirstAsync(x => x.Id == item.Id, cancellationToken);
        this.context.Credentials
            .Entry(targetCredential)
            .CurrentValues
            .SetValues(item);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
