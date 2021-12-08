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
|------------------------|---------------------------------------------------------------|-----------------------------------------------------|
| Performance            | worse over large data                                         | stable over large data                              |
| Duplicate/Skipped data | always possible if data gets updated between page navigations | guaranteed no duplication/skipping if used properly |
| Pages                  | can jump between pages                                        | can only go to first/previous/next/last             |

## Usage

[TODO]
