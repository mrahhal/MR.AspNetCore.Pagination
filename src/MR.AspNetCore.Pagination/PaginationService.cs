using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MR.EntityFrameworkCore.KeysetPagination;

namespace MR.AspNetCore.Pagination;

/// <summary>
/// Provides pagination services.
/// </summary>
public interface IPaginationService
{
	/// <summary>
	/// Paginates data using keyset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The queryable source.</param>
	/// <param name="builderAction">An action that builds the keyset.</param>
	/// <param name="getReferenceAsync">A func to load the reference from its id.</param>
	/// <param name="map">A map func to convert <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The keyset pagination result.</returns>
	Task<KeysetPaginationResult<TOut>> KeysetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		KeysetQueryModel queryModel)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates data using keyset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The queryable source.</param>
	/// <param name="builderAction">An action that builds the keyset.</param>
	/// <param name="getReferenceAsync">A func to load the reference from its id.</param>
	/// <param name="map">A map func to convert <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The keyset pagination result.</returns>
	Task<KeysetPaginationResult<TOut>> KeysetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		int? pageSize = null)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates data using offset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The queryable source.</param>
	/// <param name="map">A map func to convert <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The offset pagination result.</returns>
	Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		OffsetQueryModel queryModel)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates data using offset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The queryable source.</param>
	/// <param name="map">A map func to convert <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The offset pagination result.</returns>
	Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		int? pageSize = null)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates in memory data using offset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The in memory data source.</param>
	/// <param name="map">A func to map from <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The offset pagination result.</returns>
	OffsetPaginationResult<TOut> OffsetPaginate<T, TOut>(
		IReadOnlyList<T> source,
		Func<T, TOut> map,
		OffsetQueryModel queryModel)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates in memory data using offset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The in memory data source.</param>
	/// <param name="map">A func to map from <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The offset pagination result.</returns>
	OffsetPaginationResult<TOut> OffsetPaginate<T, TOut>(
		IReadOnlyList<T> source,
		Func<T, TOut> map,
		int? pageSize = null)
		where T : class
		where TOut : class;
}

/// <summary>
/// Provides extensions over <see cref="IPaginationService"/>.
/// </summary>
public static class PaginationServiceExtensions
{
	/// <summary>
	/// Paginates data using keyset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The queryable source.</param>
	/// <param name="builderAction">An action that builds the keyset.</param>
	/// <param name="getReferenceAsync">A func to load the reference from its id.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The keyset pagination result.</returns>
	public static Task<KeysetPaginationResult<T>> KeysetPaginateAsync<T>(
		this IPaginationService @this,
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		KeysetQueryModel queryModel)
		where T : class
		=> @this.KeysetPaginateAsync(source, builderAction, getReferenceAsync, query => query, queryModel);

	/// <summary>
	/// Paginates data using keyset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The queryable source.</param>
	/// <param name="builderAction">An action that builds the keyset.</param>
	/// <param name="getReferenceAsync">A func to load the reference from its id.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The keyset pagination result.</returns>
	public static Task<KeysetPaginationResult<T>> KeysetPaginateAsync<T>(
		this IPaginationService @this,
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		int? pageSize = null)
		where T : class
		=> @this.KeysetPaginateAsync(source, builderAction, getReferenceAsync, query => query, pageSize);

	/// <summary>
	/// Paginates data using offset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The queryable source.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The offset pagination result.</returns>
	public static Task<OffsetPaginationResult<T>> OffsetPaginateAsync<T>(
		this IPaginationService @this,
		IQueryable<T> source,
		OffsetQueryModel queryModel)
		where T : class
		=> @this.OffsetPaginateAsync(source, query => query, queryModel);

	/// <summary>
	/// Paginates data using offset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The queryable source.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The offset pagination result.</returns>
	public static Task<OffsetPaginationResult<T>> OffsetPaginateAsync<T>(
		this IPaginationService @this,
		IQueryable<T> source,
		int? pageSize = null)
		where T : class
		=> @this.OffsetPaginateAsync(source, query => query, pageSize);

	/// <summary>
	/// Paginates in memory data using offset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The in memory data source.</param>
	/// <param name="queryModel">The pagination query model.</param>
	/// <returns>The offset pagination result.</returns>
	public static OffsetPaginationResult<T> OffsetPaginate<T>(
		this IPaginationService @this,
		IReadOnlyList<T> source,
		OffsetQueryModel queryModel)
		where T : class
		=> @this.OffsetPaginate(source, x => x, queryModel);

	/// <summary>
	/// Paginates in memory data using offset pagination with a model parsed from the request's query.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <param name="this">The <see cref="IPaginationService"/> instance.</param>
	/// <param name="source">The in memory data source.</param>
	/// <param name="pageSize">The page size. This takes priority over all other sources.</param>
	/// <returns>The offset pagination result.</returns>
	public static OffsetPaginationResult<T> OffsetPaginate<T>(
		this IPaginationService @this,
		IReadOnlyList<T> source,
		int? pageSize = null)
		where T : class
		=> @this.OffsetPaginate(source, x => x, pageSize);
}

/// <summary>
/// Default implementation of <see cref="IPaginationService"/>.
/// </summary>
public class PaginationService : IPaginationService
{
	private readonly HttpContext _httpContext;
	private readonly PaginationOptions _options;

	/// <summary>
	/// Creates a new instance of <see cref="PaginationService"/>.
	/// </summary>
	public PaginationService(
		IHttpContextAccessor httpContextAccessor,
		IOptions<PaginationOptions> options)
	{
		_httpContext = httpContextAccessor?.HttpContext ?? throw new InvalidOperationException("HttpContext is required.");
		_options = options.Value;
	}

	/// <inheritdoc/>
	public async Task<KeysetPaginationResult<TOut>> KeysetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		KeysetQueryModel queryModel)
		where T : class
		where TOut : class
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (builderAction == null) throw new ArgumentNullException(nameof(builderAction));
		if (getReferenceAsync == null) throw new ArgumentNullException(nameof(getReferenceAsync));
		if (map == null) throw new ArgumentNullException(nameof(map));

		var query = source;
		var pageSize = queryModel.Size ?? _options.DefaultSize;

		var totalCount = await query.CountAsync();

		var data = default(List<TOut>);
		KeysetPaginationContext<T> keysetContext;

		if (queryModel.Last)
		{
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Backward);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}
		else if (queryModel.After != null)
		{
			var reference = await getReferenceAsync(queryModel.After);
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Forward, reference);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}
		else if (queryModel.Before != null)
		{
			var reference = await getReferenceAsync(queryModel.Before);
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Backward, reference);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}
		else
		{
			// First page
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Forward);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}

		keysetContext.EnsureCorrectOrder(data);

		var hasPrevious = await keysetContext.HasPreviousAsync(data);
		var hasNext = await keysetContext.HasNextAsync(data);

		return new KeysetPaginationResult<TOut>(data, totalCount, pageSize, hasPrevious, hasNext);
	}

	/// <inheritdoc/>
	public Task<KeysetPaginationResult<TOut>> KeysetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<string, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		int? pageSize = null)
		where T : class
		where TOut : class
	{
		var queryModel = ParseKeysetQueryModel(
			_httpContext.Request.Query,
			pageSize);
		return KeysetPaginateAsync(source, builderAction, getReferenceAsync, map, queryModel);
	}

	/// <inheritdoc/>
	public async Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		OffsetQueryModel queryModel)
		where T : class
		where TOut : class
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (map == null) throw new ArgumentNullException(nameof(map));

		var query = source;
		var pageSize = queryModel.Size ?? _options.DefaultSize;
		var page = queryModel.Page;

		var totalCount = await query.CountAsync();

		query = source.Skip((page - 1) * pageSize).Take(pageSize);

		var data = await query.ApplyMapper(map).ToListAsync();

		return CreateOffsetPaginationResult(
			data,
			totalCount,
			pageSize,
			page);
	}

	/// <inheritdoc/>
	public Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map,
		int? pageSize = null)
		where T : class
		where TOut : class
	{
		var queryModel = ParseOffsetQueryModel(
			_httpContext.Request.Query,
			pageSize);
		return OffsetPaginateAsync(source, map, queryModel);
	}

	/// <inheritdoc/>
	public OffsetPaginationResult<TOut> OffsetPaginate<T, TOut>(
		IReadOnlyList<T> source,
		Func<T, TOut> map,
		OffsetQueryModel queryModel)
		where T : class
		where TOut : class
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (map == null) throw new ArgumentNullException(nameof(map));

		var pageSize = queryModel.Size ?? _options.DefaultSize;
		var page = queryModel.Page;

		var totalCount = source.Count;
		var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

		var data = Page(source, page, pageSize)
			.Select(x => map(x))
			.ToList();

		return CreateOffsetPaginationResult(
			data,
			totalCount,
			pageSize,
			page);
	}

	/// <inheritdoc/>
	public OffsetPaginationResult<TOut> OffsetPaginate<T, TOut>(
		IReadOnlyList<T> source,
		Func<T, TOut> map,
		int? pageSize = null)
		where T : class
		where TOut : class
	{
		var queryModel = ParseOffsetQueryModel(
			_httpContext.Request.Query,
			pageSize);
		return OffsetPaginate(source, map, queryModel);
	}

	private OffsetPaginationResult<T> CreateOffsetPaginationResult<T>(
		IReadOnlyList<T> data,
		int totalCount,
		int pageSize,
		int page)
	{
		var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

		return new OffsetPaginationResult<T>(
			data,
			totalCount,
			pageSize,
			page,
			pageCount);
	}

	private IEnumerable<T> Page<T>(IReadOnlyList<T> source, int page, int pageSize)
	{
		var data = new List<T>(capacity: pageSize);

		var startOffset = (page - 1) * pageSize;
		for (var i = startOffset; i < startOffset + pageSize; i++)
		{
			if (i >= source.Count) break;
			data.Add(source[i]);
		}

		return data;
	}

	private int? ResolvePageSize(
		IQueryCollection requestQuery,
		int? argumentPageSize)
	{
		if (argumentPageSize.HasValue)
		{
			return argumentPageSize;
		}

		var pageSizeFromQuery = default(int?);
		if (_options.CanChangeSizeFromQuery && requestQuery.ContainsKey(_options.PageSizeQueryParameterName))
		{
			var pageSizeString = requestQuery[_options.PageSizeQueryParameterName][0];
			pageSizeFromQuery = int.Parse(pageSizeString);

			if (pageSizeFromQuery <= 0)
			{
				pageSizeFromQuery = _options.DefaultSize;
			}
			if (pageSizeFromQuery > _options.MaxSize)
			{
				pageSizeFromQuery = _options.MaxSize;
			}
		}

		return pageSizeFromQuery;
	}

	private KeysetQueryModel ParseKeysetQueryModel(
		IQueryCollection requestQuery,
		int? argumentSize)
	{
		var model = new KeysetQueryModel
		{
			Size = ResolvePageSize(requestQuery, argumentSize),
		};

		if (requestQuery.ContainsKey(_options.FirstQueryParameterName))
		{
			var first = requestQuery[_options.FirstQueryParameterName][0];
			if (!bool.TryParse(first, out var firstBoolean))
			{
				firstBoolean = false;
			}
			model.First = firstBoolean;
		}

		if (requestQuery.ContainsKey(_options.BeforeQueryParameterName))
		{
			model.Before = requestQuery[_options.BeforeQueryParameterName][0];
		}

		if (requestQuery.ContainsKey(_options.AfterQueryParameterName))
		{
			model.After = requestQuery[_options.AfterQueryParameterName][0];
		}

		if (requestQuery.ContainsKey(_options.LastQueryParameterName))
		{
			var last = requestQuery[_options.LastQueryParameterName][0];
			if (!bool.TryParse(last, out var lastBoolean))
			{
				lastBoolean = false;
			}
			model.Last = lastBoolean;
		}

		return model;
	}

	private OffsetQueryModel ParseOffsetQueryModel(
		IQueryCollection requestQuery,
		int? argumentSize)
	{
		var model = new OffsetQueryModel
		{
			Size = ResolvePageSize(requestQuery, argumentSize),
		};

		if (requestQuery.ContainsKey(_options.PageQueryParameterName))
		{
			var page = requestQuery[_options.PageQueryParameterName][0];
			if (int.TryParse(page, out var pageIntValue))
			{
				model.Page = pageIntValue;
			}

			if (model.Page < 1)
			{
				// Guard against invalid page values by returning 1st page.
				model.Page = 1;
			}
		}

		return model;
	}
}
