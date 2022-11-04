using AutoMapper;
using AutoMapper.QueryableExtensions;
using Basic.Dtos;
using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;

namespace Basic.Pages
{
	public class Keyset3Model : PageModel
	{
		private readonly AppDbContext _dbContext;
		private readonly IPaginationService _paginationService;
		private readonly IMapper _mapper;

		public Keyset3Model(
			AppDbContext dbContext,
			IPaginationService paginationService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_paginationService = paginationService;
			_mapper = mapper;
		}

		public KeysetPaginationResult<Post> Posts { get; set; }

		public async Task OnGet()
		{
			var query = _dbContext.Posts;

			Posts = await _paginationService.KeysetPaginateAsync(
				query,
				b => b.Descending(x => x.Created),
				async id => await _dbContext.Posts.FindAsync(Guid.Parse(id)));
		}
	}
}
