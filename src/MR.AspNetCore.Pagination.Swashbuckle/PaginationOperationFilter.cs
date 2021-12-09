using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MR.AspNetCore.Pagination.Swashbuckle;

/// <summary>
/// An <see cref="IOperationFilter"/> that adds the default set of parameters for actions returning pagination results.
/// </summary>
public class PaginationOperationFilter : IOperationFilter
{
	private readonly PaginationOptions _paginationOptions;

	public PaginationOperationFilter(
		IOptions<PaginationOptions> paginationOptions)
	{
		_paginationOptions = paginationOptions.Value;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var boolSchema = context.SchemaGenerator.GenerateSchema(typeof(bool), context.SchemaRepository);
		var intSchema = context.SchemaGenerator.GenerateSchema(typeof(int), context.SchemaRepository);

		if (PaginationActionDetector.IsKeysetPaginationResultAction(context.MethodInfo, out _))
		{
			CreateParameter(_paginationOptions.FirstQueryParameterName, "true if you want the first page", boolSchema);
			CreateParameter(_paginationOptions.BeforeQueryParameterName, "Id of the reference entity you want results before");
			CreateParameter(_paginationOptions.AfterQueryParameterName, "Id of the reference entity you want results after");
			CreateParameter(_paginationOptions.LastQueryParameterName, "true if you want the last page", boolSchema);
		}
		else if (PaginationActionDetector.IsOffsetPaginationResultAction(context.MethodInfo, out _))
		{
			CreateParameter(_paginationOptions.PageQueryParameterName, "The page", intSchema);
		}

		void CreateParameter(string name, string description, OpenApiSchema? schema = null)
		{
			operation.Parameters.Add(new OpenApiParameter
			{
				Required = false,
				In = ParameterLocation.Query,
				Name = name,
				Description = description,
				Schema = schema,
			});
		}
	}
}
