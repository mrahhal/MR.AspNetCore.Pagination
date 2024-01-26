# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## Unreleased

### Improved

- Require HttpContext only where needed instead of throwing on ctor ([#11](https://github.com/mrahhal/MR.AspNetCore.Pagination/pull/11))

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v2.1.0...HEAD)

## 2.1.0 - 2023-05-13

### Added

- Add support for prebuilt keyset query definitions

### Other

- Update MR.EntityFrameworkCore.KeysetPagination package version to v1.3.0

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v2.0.1...v2.1.0)

## 2.0.1 - 2023-02-10

### Fixed

- Properly annotate the `getReferenceAsync` delegate to allow nulls, which fixes the mismatch in analysis ([#7](https://github.com/mrahhal/MR.AspNetCore.Pagination/pull/7))

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v2.0.0...v2.0.1)

## 2.0.0 - 2022-11-05

This version introduces breaking changes. Make sure to read on them in the Changed section below.

### Fixed

- Enforce returning the first page when the reference is null in an after/before request

### Added

- Add overloads that accept the query model as an argument as opposed to being parsed from the request query ([#3](https://github.com/mrahhal/MR.AspNetCore.Pagination/issues/3))
- Add `PageCount`, a commonly computed value to `OffsetPaginationResult`

### Changed

- Remove auto parsing of ints for after/before params ([#5](https://github.com/mrahhal/MR.AspNetCore.Pagination/issues/5))
- Rename `SizeQueryParameterName` to `PageSizeQueryParameterName` (the default value remains `"size"`)

### Other

- Update MR.EntityFrameworkCore.KeysetPagination dependency

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v1.0.1...v2.0.0)

## 1.0.1 - 2022-02-18

- Update MR.EntityFrameworkCore.KeysetPagination dependency

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v1.0.0...v1.0.1)

## 1.0.0 - 2021-12-12

- Move `PaginationActionDetector` to main package
- Add `ConfigurePagination` for swagger and make `AddPaginationOperationFilter` obsolete
- Add efficient offset pagination for in memory list of data
- Allow overriding page size on the method call level

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v0.1.0...v1.0.0)
