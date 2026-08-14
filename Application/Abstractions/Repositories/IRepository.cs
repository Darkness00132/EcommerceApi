using Api.Contracts.Common;
using Application.Common.Pagination;
using Domain.Common;
using System.Linq.Expressions;

namespace Application.Abstractions.Repositories;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TEntity?> SingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TResponse?> ProjectToSingleOrDefaultAsync<TResponse>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TResponse : class;

    Task<PagedResult<TResponse>> ProjectToPagedWithPaginationAsync<TResponse>(
        PaginationRequest pagination,
        Expression<Func<TEntity, object>> orderBy,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
        where TResponse : class;

    Task<IReadOnlyList<TResponse>> ProjectToPagedAsync<TResponse>(
        Expression<Func<TEntity, object>> orderBy,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
        where TResponse : class;

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);
}