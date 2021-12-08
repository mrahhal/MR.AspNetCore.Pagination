namespace MR.AspNetCore.Pagination;

/// <summary>
/// Provides programmatic configuration used by pagination services.
/// </summary>
public class PaginationOptions
{
	/// <summary>
	/// Gets or sets the query parameter name for first.
	/// Defaults to "first".
	/// </summary>
	public string FirstQueryParameterName { get; set; } = "first";

	/// <summary>
	/// Gets or sets the query parameter name for before.
	/// Defaults to "before".
	/// </summary>
	public string BeforeQueryParameterName { get; set; } = "before";

	/// <summary>
	/// Gets or sets the query parameter name for after.
	/// Defaults to "after".
	/// </summary>
	public string AfterQueryParameterName { get; set; } = "after";

	/// <summary>
	/// Gets or sets the query parameter name for last.
	/// Defaults to "last".
	/// </summary>
	public string LastQueryParameterName { get; set; } = "last";

	/// <summary>
	/// Gets or sets the query parameter name for page.
	/// Defaults to "page".
	/// </summary>
	public string PageQueryParameterName { get; set; } = "page";

	/// <summary>
	/// Gets or sets the default page size.
	/// Defaults to 20.
	/// </summary>
	public int DefaultSize { get; set; } = 20;

	/// <summary>
	/// Gets or sets the maximum page size that can be specified. Should throw if this size is exceeded.
	/// Defaults to 100.
	/// </summary>
	public int MaxSize { get; set; } = 100;

	/// <summary>
	/// Determines whether the size query param is used to override the default size.
	/// Defaults to false.
	/// </summary>
	public bool CanChangeSizeFromQuery { get; set; } = false;

	/// <summary>
	/// Gets or sets the query parameter name for page size.
	/// Defaults to "size".
	/// </summary>
	public string SizeQueryParameterName { get; set; } = "size";
}
