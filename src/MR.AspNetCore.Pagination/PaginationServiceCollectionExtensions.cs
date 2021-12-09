using Microsoft.Extensions.DependencyInjection.Extensions;
using MR.AspNetCore.Pagination;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up pagination services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class PaginationServiceCollectionExtensions
{
	private static IServiceCollection AddPaginationCore(
		this IServiceCollection services,
		Action<PaginationOptions> configure)
	{
		services.AddHttpContextAccessor();
		services.Configure(configure);
		services.TryAddScoped<IPaginationService, PaginationService>();
		return services;
	}

	/// <summary>
	/// Adds pagination services to the specified <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
	public static IServiceCollection AddPagination(this IServiceCollection services)
	{
		if (services == null)
		{
			throw new ArgumentNullException(nameof(services));
		}

		return AddPaginationCore(services, o => { });
	}

	/// <summary>
	/// Adds pagination services to the specified <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
	/// <param name="configure">An action delegate to configure the provided <see cref="PaginationOptions"/>.</param>
	public static IServiceCollection AddPagination(this IServiceCollection services, Action<PaginationOptions> configure)
	{
		if (services == null)
		{
			throw new ArgumentNullException(nameof(services));
		}
		if (configure == null)
		{
			throw new ArgumentNullException(nameof(configure));
		}

		return AddPaginationCore(services, configure);
	}
}
