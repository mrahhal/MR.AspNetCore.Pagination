namespace MR.AspNetCore.Pagination.TestModels;

public class Order
{
	public int Id { get; set; }

	public int? AnotherId { get; set; }

	public DateTime Created { get; set; }
}

public class OrderDto
{
	public int Id { get; set; }

	public int? AnotherId { get; set; }

	public DateTime Created { get; set; }
}
