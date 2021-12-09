using Microsoft.EntityFrameworkCore;

namespace MR.AspNetCore.Pagination.TestModels;

public class TestDbContext : DbContext
{
	public TestDbContext(
		DbContextOptions<TestDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	}

	public DbSet<Order> Orders { get; set; }
}
