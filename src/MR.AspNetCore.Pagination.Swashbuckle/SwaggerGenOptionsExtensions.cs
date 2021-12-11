using MR.AspNetCore.Pagination.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions over <see cref="SwaggerGenOptions"/>.
/// </summary>
public static class SwaggerGenOptionsExtensions
{
	/// <summary>
	/// Configures pagination related services for swagger.
	/// </summary>
	public static void ConfigurePagination(this SwaggerGenOptions @this)
	{
		@this.OperationFilter<PaginationOperationFilter>();
	}

	/// <summary>
	/// Configures pagination related services for swagger.
	/// </summary>
	[Obsolete("Use ConfigurePagination instead.")]
	public static void AddPaginationOperationFilter(this SwaggerGenOptions @this)
	{
		@this.ConfigurePagination();
	}
}
