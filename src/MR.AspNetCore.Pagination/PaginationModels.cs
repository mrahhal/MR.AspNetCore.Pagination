namespace MR.AspNetCore.Pagination;

public abstract class QueryModelBase
{
	public int? Size { get; set; }
}

/// <summary>
/// The query model for a keyset pagination.
/// </summary>
/// <remarks>
/// Specifying conflicting options leads to undefined behavior.
/// </remarks>
public class KeysetQueryModel : QueryModelBase
{
	public bool First { get; set; }

	public string? Before { get; set; }

	public string? After { get; set; }

	public bool Last { get; set; }
}

/// <summary>
/// The query model for an offset pagination.
/// </summary>
public class OffsetQueryModel : QueryModelBase
{
	public int Page { get; set; } = 1;
}
