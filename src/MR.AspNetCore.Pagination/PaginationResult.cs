namespace MR.AspNetCore.Pagination;

/// <summary>
/// Represents the result of the keyset pagination.
/// </summary>
/// <typeparam name="T">The type of the data list item.</typeparam>
/// <param name="Data">The data list.</param>
/// <param name="Count">The total count of the data.</param>
/// <param name="HasPrevious">Whether there's previous data to the list.</param>
/// <param name="HasNext">Whether there's next data to the list.</param>
/// <param name="PageSize">The size of the page. This is different from the actual size of <see cref="Data"/>.</param>
public record KeysetPaginationResult<T>(
	List<T> Data,
	int Count,
	int PageSize,
	bool HasPrevious,
	bool HasNext);

/// <summary>
/// Represents the result of the offset pagination.
/// </summary>
/// <typeparam name="T">The type of the data list item.</typeparam>
/// <param name="Data">The data list.</param>
/// <param name="Count">The total count of the data.</param>
/// <param name="PageSize">The size of the page. This is different from the actual size of <see cref="Data"/>.</param>
public record OffsetPaginationResult<T>(
	List<T> Data,
	int Count,
	int PageSize);
