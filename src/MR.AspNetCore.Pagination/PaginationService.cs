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
	/// <param name="getReferenceAsync">A func that gets the reference by id.</param>
	/// <param name="map">A func that decorates the source to map from <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <returns>The keyset pagination result.</returns>
	Task<KeysetPaginationResult<TOut>> KeysetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Action<KeysetPaginationBuilder<T>> builderAction,
		Func<object, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map)
		where T : class
		where TOut : class;

	/// <summary>
	/// Paginates data using offset pagination.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	/// <typeparam name="TOut">The type of the transformed object.</typeparam>
	/// <param name="source">The queryable source.</param>
	/// <param name="map">A func that decorates the source to map from <typeparamref name="T"/> to <typeparamref name="TOut"/>.</param>
	/// <returns>The offset pagination result.</returns>
	Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map)
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
	/// <param name="getReferenceAsync">A func that gets the reference by id.</param>
	/// <returns>The keyset pagination result.</returns>
	public static Task<KeysetPaginationResult<T>> KeysetPaginateAsync<T>(
		this IPaginationService @this,
		IQueryable<T> source,
		Func<object, Task<T>> getReferenceAsync,
		Action<KeysetPaginationBuilder<T>> builderAction)
		where T : class
		=> @this.KeysetPaginateAsync(source, builderAction, getReferenceAsync, query => query);
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
		Func<object, Task<T>> getReferenceAsync,
		Func<IQueryable<T>, IQueryable<TOut>> map)
		where T : class
		where TOut : class
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (builderAction == null) throw new ArgumentNullException(nameof(builderAction));
		if (getReferenceAsync == null) throw new ArgumentNullException(nameof(getReferenceAsync));
		if (map == null) throw new ArgumentNullException(nameof(map));

		var model = ParseKeysetQueryModel(_httpContext.Request.Query);
		var query = source;
		var pageSize = ResolvePageSize(model);

		var count = await query.CountAsync();

		var data = default(List<TOut>);
		KeysetPaginationContext<T> keysetContext;

		if (model.Last)
		{
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Backward);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}
		else if (model.After != null)
		{
			var reference = await getReferenceAsync(model.After);
			keysetContext = query.KeysetPaginate(builderAction, KeysetPaginationDirection.Forward, reference);
			data = await keysetContext.Query
			  .Take(pageSize)
			  .ApplyMapper(map)
			  .ToListAsync();
		}
		else if (model.Before != null)
		{
			var reference = await getReferenceAsync(model.Before);
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

		return new KeysetPaginationResult<TOut>(data, count, pageSize, hasPrevious, hasNext);
	}

	/// <inheritdoc/>
	public async Task<OffsetPaginationResult<TOut>> OffsetPaginateAsync<T, TOut>(
		IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map)
		where T : class
		where TOut : class
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (map == null) throw new ArgumentNullException(nameof(map));

		var model = ParseOffsetQueryModel(_httpContext.Request.Query);
		var query = source;
		var pageSize = ResolvePageSize(model);
		var page = model.Page;
		if (page < 1)
		{
			// Guard against invalid page values by returning 1st page.
			page = 1;
		}

		var count = await query.CountAsync();

		query = source.Skip((page - 1) * pageSize).Take(pageSize);

		var data = await query.ApplyMapper(map).ToListAsync();

		return new OffsetPaginationResult<TOut>(data, count, pageSize);
	}

	private int ResolvePageSize(QueryModelBase model)
	{
		if (model.Size != null) return model.Size.Value;
		return _options.DefaultSize;
	}

	private KeysetQueryModel ParseKeysetQueryModel(IQueryCollection requestQuery)
	{
		var model = new KeysetQueryModel();

		ParseIntoQueryModelBase(requestQuery, model);

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
			var before = requestQuery[_options.BeforeQueryParameterName][0];
			if (int.TryParse(before, out var beforeIntValue))
			{
				model.Before = beforeIntValue;
			}
			else
			{
				model.Before = before;
			}
		}

		if (requestQuery.ContainsKey(_options.AfterQueryParameterName))
		{
			var after = requestQuery[_options.AfterQueryParameterName][0];
			if (int.TryParse(after, out var afterIntValue))
			{
				model.After = afterIntValue;
			}
			else
			{
				model.After = after;
			}
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

		if (_options.CanChangeSizeFromQuery && requestQuery.ContainsKey(_options.SizeQueryParameterName))
		{
			var size = requestQuery[_options.SizeQueryParameterName][0];
			if (int.TryParse(size, out var sizeIntValue))
			{
				model.Size = sizeIntValue;
			}
			else
			{
				model.Size = _options.DefaultSize;
			}
		}

		return model;
	}

	private OffsetQueryModel ParseOffsetQueryModel(IQueryCollection requestQuery)
	{
		var model = new OffsetQueryModel();

		ParseIntoQueryModelBase(requestQuery, model);

		if (requestQuery.ContainsKey(_options.PageQueryParameterName))
		{
			var page = requestQuery[_options.PageQueryParameterName][0];
			if (int.TryParse(page, out var pageIntValue))
			{
				model.Page = pageIntValue;
			}
		}

		return model;
	}

	private void ParseIntoQueryModelBase(IQueryCollection requestQuery, QueryModelBase model)
	{
		if (_options.CanChangeSizeFromQuery && requestQuery.ContainsKey(_options.SizeQueryParameterName))
		{
			var size = requestQuery[_options.SizeQueryParameterName][0];
			if (int.TryParse(size, out var sizeIntValue))
			{
				model.Size = sizeIntValue;
			}
		}
	}

	private abstract class QueryModelBase
	{
		public int? Size { get; set; }
	}

	private class KeysetQueryModel : QueryModelBase
	{
		public bool First { get; set; }

		public object? Before { get; set; }

		public object? After { get; set; }

		public bool Last { get; set; }
	}

	private class OffsetQueryModel : QueryModelBase
	{
		public int Page { get; set; } = 1;
	}
}
