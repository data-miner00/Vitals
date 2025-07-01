namespace Vitals.Core.Repositories;

using Vitals.Core.Models;

public interface IVoteRepository : IRepository<Vote>
{
    Task<Vote?> GetByPostId(int postId, CancellationToken cancellationToken);
}
