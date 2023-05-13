using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using MR.AspNetCore.Pagination.TestModels;
using MR.EntityFrameworkCore.KeysetPagination;
using Xunit;

namespace MR.AspNetCore.Pagination.Tests;

public class PaginationServiceTest : IClassFixture<DatabaseFixture>
{
	private const string PageParameter = "page";

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

	protected List<Order> CreateOrders(int count) => Enumerable.Range(1, count)
		.Select(x => new Order { Id = x })
		.ToList();

	public class Basic : PaginationServiceTest
	{
		public Basic(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async Task KeysetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(int.Parse(id)));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}

		[Fact]
		public async Task KeysetPaginateAsync_Prebuilt()
		{
			var query = DbContext.Orders;
			var keysetQuery = KeysetQuery.Build<Order>(b => b.Ascending(o => o.Id));

			var result = await Service.KeysetPaginateAsync(
				query,
				keysetQuery,
				async id => await DbContext.Orders.FindAsync(int.Parse(id)));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}

		[Fact]
		public async Task OffsetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.OffsetPaginateAsync(
				query);

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}

		[Fact]
		public void OffsetPaginate()
		{
			var defaultPageSize = new PaginationOptions().DefaultSize;
			var orders = CreateOrders(defaultPageSize + 5);

			var result = Service.OffsetPaginate(
				orders);

			result.Data.Should().HaveCount(defaultPageSize);
		}
	}

	public class Map : PaginationServiceTest
	{
		public Map(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async Task KeysetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(int.Parse(id)),
				query => query.Select(x => new OrderDto { Id = x.Id }));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
			result.Data[0].Should().BeOfType<OrderDto>();
		}

		[Fact]
		public async Task OffsetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.OffsetPaginateAsync(
				query,
				query => query.Select(x => new OrderDto { Id = x.Id }));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
			result.Data[0].Should().BeOfType<OrderDto>();
		}

		[Fact]
		public void OffsetPaginate()
		{
			var defaultPageSize = new PaginationOptions().DefaultSize;
			var orders = CreateOrders(defaultPageSize + 5);

			var result = Service.OffsetPaginate(
				orders,
				x => new OrderDto { Id = x.Id });

			result.Data.Should().HaveCount(defaultPageSize);
			result.Data[0].Should().BeOfType<OrderDto>();
		}
	}

	public class PageArgument : PaginationServiceTest
	{
		public PageArgument(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async Task KeysetPaginateAsync()
		{
			var query = DbContext.Orders;

			var pageSize = 2;
			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(int.Parse(id)),
				pageSize);

			result.Data.Should().HaveCount(pageSize);
		}

		[Fact]
		public async Task OffsetPaginateAsync()
		{
			var query = DbContext.Orders;

			var pageSize = 2;
			var result = await Service.OffsetPaginateAsync(
				query,
				pageSize);

			result.Data.Should().HaveCount(pageSize);
		}

		[Fact]
		public void OffsetPaginate()
		{
			var orders = CreateOrders(5);

			var pageSize = 2;
			var result = Service.OffsetPaginate(
				orders,
				pageSize);

			result.Data.Should().HaveCount(pageSize);
		}
	}

	public class Page2 : PaginationServiceTest
	{
		public Page2(DatabaseFixture fixture) : base(fixture)
		{
		}

		protected override IQueryCollection CreateQuery()
		{
			return new QueryCollection(new Dictionary<string, StringValues>
			{
				{ PageParameter, "2" },
			});
		}

		[Fact]
		public void OffsetPaginate()
		{
			var orders = CreateOrders(5);

			var result = Service.OffsetPaginate(
				orders,
				pageSize: 2);

			result.Data.Should().HaveCount(2);
			result.Data[0].Id.Should().Be(3);
			result.Data[1].Id.Should().Be(4);
		}

		[Fact]
		public void OffsetPaginate_ActualSizeLessThanPageSize()
		{
			var orders = CreateOrders(5);

			var result = Service.OffsetPaginate(
				orders,
				pageSize: 4);

			result.Data.Should().HaveCount(1);
			result.Data[0].Id.Should().Be(5);
		}
	}

	public class After : PaginationServiceTest
	{
		public After(DatabaseFixture fixture) : base(fixture)
		{
		}

		protected override IQueryCollection CreateQuery()
		{
			var query = new QueryCollection(new Dictionary<string, StringValues>
			{
				[new PaginationOptions().AfterQueryParameterName] = 1.ToString(),
			});

			return query;
		}

		[Fact]
		public async Task KeysetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(int.Parse(id)));

			var defaultPageSize = new PaginationOptions().DefaultSize;
			result.Data.Should().HaveCount(defaultPageSize);
		}

		[Fact]
		public async Task KeysetPaginateAsync_NoProperParsing_Throws()
		{
			var query = DbContext.Orders;

			await Assert.ThrowsAnyAsync<Exception>(async () => await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(id)));
		}
	}

	public class ArgumentQueryModel : PaginationServiceTest
	{
		public ArgumentQueryModel(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async Task KeysetPaginateAsync()
		{
			var query = DbContext.Orders;

			var result = await Service.KeysetPaginateAsync(
				query,
				b => b.Ascending(o => o.Id),
				async id => await DbContext.Orders.FindAsync(int.Parse(id)),
				queryModel: new KeysetQueryModel
				{
					First = true,
					Size = 10,
				});

			result.Data.Should().HaveCount(10);
		}
	}
}
