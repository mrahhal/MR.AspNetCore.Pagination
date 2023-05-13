using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;
using MR.EntityFrameworkCore.KeysetPagination;

namespace Basic.Pages
{
	public class IndexModel : PageModel
	{
		private static readonly KeysetQueryDefinition<User> _usersKeysetQuery =
			KeysetQuery.Build<User>(b => b.Descending(x => x.Created));

		private readonly AppDbContext _dbContext;
		private readonly IPaginationService _paginationService;

		public IndexModel(
			AppDbContext dbContext,
			IPaginationService paginationService)
		{
			_dbContext = dbContext;
			_paginationService = paginationService;
		}

		public KeysetPaginationResult<User> Users { get; set; }

		public async Task OnGet()
		{
#nullable enable
			var query = _dbContext.Users;

			Users = await _paginationService.KeysetPaginateAsync(
				query,
				_usersKeysetQuery,
				async id => await _dbContext.Users.FindAsync(int.Parse(id)));
		}
	}
}
