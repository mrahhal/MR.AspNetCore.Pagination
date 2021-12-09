using MR.AspNetCore.Pagination.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
	public static void AddPaginationOperationFilter(this SwaggerGenOptions @this)
	{
		@this.OperationFilter<PaginationOperationFilter>();
	}
}
