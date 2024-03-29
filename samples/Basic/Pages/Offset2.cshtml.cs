﻿using AutoMapper;
using Basic.Dtos;
using Basic.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MR.AspNetCore.Pagination;

namespace Basic.Pages
{
	public class Offset2Model : PageModel
	{
		private readonly AppDbContext _dbContext;
		private readonly IPaginationService _paginationService;
		private readonly IMapper _mapper;

		public Offset2Model(
			AppDbContext dbContext,
			IPaginationService paginationService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_paginationService = paginationService;
			_mapper = mapper;
		}

		public OffsetPaginationResult<UserDto> Users { get; set; }

		public async Task OnGet()
		{
			var query = _dbContext.Users.OrderByDescending(x => x.Created);

			Users = await _paginationService.OffsetPaginateAsync(
				query,
				query => query.Select(x => new UserDto { Id = x.Id, Name = x.Name, Created = x.Created }));
		}
	}
}
