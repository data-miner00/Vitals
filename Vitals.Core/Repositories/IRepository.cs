namespace Vitals.Core.Repositories;

using System.Collections.Generic;

/// <summary>
/// The base interface for repositories.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Add a new item into the database.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task AddAsync(T item, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all the available data from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entire list of data.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the item by ID.
    /// </summary>
    /// <param name="id">The id of the item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The item itself. <c>null</c> if not found.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an item from the database with the given id.
    /// </summary>
    /// <param name="id">The id of the item to be removed.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task RemoveAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an item with the new details provided.
    /// </summary>
    /// <param name="item">The item with updated details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task UpdateAsync(T item, CancellationToken cancellationToken);

    /// <summary>
    /// Searches the <typeparamref name="T"/> entity with against a keyword.
    /// </summary>
    /// <param name="keyword">The keyword query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of found <typeparamref name="T"/>.</returns>
    Task<IEnumerable<T>> SearchAsync(string keyword, CancellationToken cancellationToken);
}
