using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;

namespace Basic.Pages
{
	public class IndexModel : PageModel
	{
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
			var query = _dbContext.Users;

			Users = await _paginationService.KeysetPaginateAsync(
				query,
				b => b.Descending(x => x.Created),
				async id => await _dbContext.Users.FindAsync(int.Parse(id)));
		}
	}
}
