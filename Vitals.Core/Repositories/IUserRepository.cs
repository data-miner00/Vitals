namespace Vitals.Core.Repositories;

using System.Threading.Tasks;
using Vitals.Core.Models;

/// <summary>
/// A repository interface for managing user entities.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user if found.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user if found.</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
