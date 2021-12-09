using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;

namespace Basic.Pages
{
	public class Offset1Model : PageModel
	{
		private readonly AppDbContext _dbContext;
		private readonly IPaginationService _paginationService;

		public Offset1Model(
			AppDbContext dbContext,
			IPaginationService paginationService)
		{
			_dbContext = dbContext;
			_paginationService = paginationService;
		}

		public OffsetPaginationResult<User> Users { get; set; }

		public async Task OnGet()
		{
			var query = _dbContext.Users.OrderByDescending(x => x.Created);

			Users = await _paginationService.OffsetPaginateAsync(query);
		}
	}
}
