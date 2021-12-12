# MR.AspNetCore.Pagination

[![CI](https://github.com/mrahhal/MR.AspNetCore.Pagination/actions/workflows/ci.yml/badge.svg)](https://github.com/mrahhal/MR.AspNetCore.Pagination/actions/workflows/ci.yml)
[![NuGet version](https://badge.fury.io/nu/MR.AspNetCore.Pagination.svg)](https://www.nuget.org/packages/MR.AspNetCore.Pagination)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Pagination for ASP.NET Core.

Supports both offset and keyset pagination over `IQueryable` for Entity Framework Core.

Keyset pagination support uses [MR.EntityFrameworkCore.KeysetPagination](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination).

## Offset vs Keyset

Keyset pagination (also known as cursor/seek pagination) is much more efficient and has stable performance over large sizes of data, but it's harder to work with than offset pagination.

|                        | Offset                                                        | Keyset                                              |
| ---------------------- | ------------------------------------------------------------- | --------------------------------------------------- |
| Performance            | worse over large data                                         | stable over large data                              |
| Duplicate/Skipped data | always possible if data gets updated between page navigations | guaranteed no duplication/skipping if used properly |
| Pages                  | can jump between pages                                        | can only go to first/previous/next/last             |

We recommend keyset pagination over offset, unless you have specific requirements for wanting to show/navigate pages.

## Usage

## MR.AspNetCore.Pagination

[![NuGet version](https://badge.fury.io/nu/MR.AspNetCore.Pagination.svg)](https://www.nuget.org/packages/MR.AspNetCore.Pagination)

Add pagination services:

```cs
services.AddPagination();
```

You can configure some default options:

```cs
services.AddPagination(options =>
{
    options.PageQueryParameterName = "p";
    // ...
});
```

Check the `PaginationOptions` class to see what you can configure.

And then just inject `IPaginationService` in your controller/page and use it.

Do a keyset pagination:

```cs
var usersPaginationResult = await _paginationService.KeysetPaginateAsync(
    _dbContext.Users,
    b => b.Descending(x => x.Created),
    async id => await _dbContext.Users.FindAsync(id));
```

Do a keyset pagination and map to dto:

```cs
// Using AutoMapper for example:
var usersPaginationResult = await _paginationService.KeysetPaginateAsync(
    _dbContext.Users,
    b => b.Descending(x => x.Created),
    async id => await _dbContext.Users.FindAsync(id),
    query => query.ProjectTo<UserDto>(_mapper.ConfigurationProvider));

// Using manual select:
var usersPaginationResult = await _paginationService.KeysetPaginateAsync(
    _dbContext.Users,
    b => b.Descending(x => x.Created),
    async id => await _dbContext.Users.FindAsync(id),
    query => query.Select(user => new UserDto(...)));
```

Do an offset pagination:

```cs
var usersPaginationResult = await _paginationService.OffsetPaginateAsync(
    _dbContext.Users.OrderByDescending(x => x.Created));
```

Do an offset pagination and map to dto:

```cs
// Using AutoMapper for example:
var usersPaginationResult = await _paginationService.OffsetPaginateAsync(
    _dbContext.Users.OrderByDescending(x => x.Created),
    query => query.ProjectTo<UserDto>(_mapper.ConfigurationProvider));

// Using manual select:
var usersPaginationResult = await _paginationService.OffsetPaginateAsync(
    _dbContext.Users.OrderByDescending(x => x.Created),
    query => query.Select(x => new UserDto(...)));
```

There's additional support for doing an offset pagination over in memory list of data:

```cs
// Assume we have an in memory list of orders.
var orders = new List<Order>();

// This does efficient offset pagination over the orders list.
var result = _paginationService.OffsetPaginate(orders);
```

There's a helper `PaginationActionDetector` class that can be used with reflection, for example in ASP.NET Core conventions, which can tell you whether the action method returns a pagination result or not. This is what the MR.AspNetCore.Pagination.Swashbuckle package uses to configure swagger for those apis.

## MR.AspNetCore.Pagination.Swashbuckle

[![NuGet version](https://badge.fury.io/nu/MR.AspNetCore.Pagination.Swashbuckle.svg)](https://www.nuget.org/packages/MR.AspNetCore.Pagination.Swashbuckle)

This is a Swashbuckle support package for MR.AspNetCore.Pagination.

Use it when you're using [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) (swagger). Actions that return keyset/offset pagination results will automatically have the query parameters (page, before, after, etc) show up in swagger ui.

When you're configuring swagger:

```cs
builder.Services.AddSwaggerGen(c =>
{
    // ...

    c.ConfigurePagination();
});
```

## Samples

Check the [samples](samples) folder for project samples.

- [Basic](samples/Basic): A quick example of using keyset and offset pagination (using razor pages).
- [ApiWithSwashbuckle](samples/ApiWithSwashbuckle): A sample that uses the Swashbuckle support package.
