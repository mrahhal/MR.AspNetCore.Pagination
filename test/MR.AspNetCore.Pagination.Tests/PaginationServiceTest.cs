using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MR.AspNetCore.Pagination.TestModels;
using Xunit;

namespace MR.AspNetCore.Pagination.Tests;

public class PaginationServiceTest : IClassFixture<DatabaseFixture>
{
	public PaginationServiceTest(DatabaseFixture fixture)
	{
		var provider = fixture.BuildForService<PaginationService>(services =>
		{
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(x => x.Request.Query).Returns(CreateQuery());
			services.Configure<PaginationOptions>(o => ConfigurePaginationOptions(o));
			services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = mockHttpContext.Object });
		});
		DbContext = provider.GetService<TestDbContext>();
		Service = provider.GetService<PaginationService>();
	}

	public TestDbContext DbContext { get; }

	public PaginationService Service { get; }

	protected virtual IQueryCollection CreateQuery()
	{
		return new QueryCollection();
	}

	protected virtual void ConfigurePaginationOptions(PaginationOptions paginationOptions)
	{
	}

	public class Basic : PaginationServiceTest
	{
		public Basic(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async Task KeysetPaginate()
		{
			var query = DbContext.Orders;

			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.FindAsync<Order>(id));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}

		[Fact]
		public async Task OffsetPaginate()
		{
			var query = DbContext.Orders;

			var result = await Service.OffsetPaginateAsync(
				query);

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}
	}
}
