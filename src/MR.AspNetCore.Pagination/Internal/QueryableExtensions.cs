namespace MR.AspNetCore.Pagination;

internal static class QueryableExtensions
{
	public static IQueryable<TOut> ApplyMapper<T, TOut>(
		this IQueryable<T> source,
		Func<IQueryable<T>, IQueryable<TOut>> map)
		where T : class
		where TOut : class
	{
		return map(source);
	}
}
