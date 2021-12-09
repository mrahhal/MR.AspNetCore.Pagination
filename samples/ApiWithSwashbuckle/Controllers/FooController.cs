using Microsoft.AspNetCore.Mvc;
using MR.AspNetCore.Pagination;

namespace ApiWithSwashbuckle.Controllers;

public class User
{
	public int Id { get; set; }

	public string Name { get; set; }
}

public class FooController
{
	[HttpGet("users1")]
	public ActionResult<KeysetPaginationResult<User>> GetUsers1()
	{
		throw new NotImplementedException();
	}

	[HttpGet("users2")]
	public Task<ActionResult<KeysetPaginationResult<User>>> GetUsers2()
	{
		throw new NotImplementedException();
	}

	[HttpGet("users3")]
	public ActionResult<OffsetPaginationResult<User>> GetUsers3()
	{
		throw new NotImplementedException();
	}

	[HttpGet("users4")]
	public Task<ActionResult<OffsetPaginationResult<User>>> GetUsers4()
	{
		throw new NotImplementedException();
	}
}
