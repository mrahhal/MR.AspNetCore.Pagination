using AutoMapper;
using AutoMapper.QueryableExtensions;
using Basic.Dtos;
using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;

namespace Basic.Pages
{
	public class Keyset2Model : PageModel
	{
		private readonly AppDbContext _dbContext;
		private readonly IPaginationService _paginationService;
		private readonly IMapper _mapper;

		public Keyset2Model(
			AppDbContext dbContext,
			IPaginationService paginationService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_paginationService = paginationService;
			_mapper = mapper;
		}

		public KeysetPaginationResult<UserDto> Users { get; set; }

		public async Task OnGet()
		{
			var query = _dbContext.Users.AsQueryable();

			Users = await _paginationService.KeysetPaginateAsync(
				query,
				b => b.Descending(x => x.Created),
				async id => await _dbContext.Users.FindAsync(id),
				query => query.ProjectTo<UserDto>(_mapper.ConfigurationProvider));
		}
	}
}
