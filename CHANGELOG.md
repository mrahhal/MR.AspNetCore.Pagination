# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## Unreleased

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v1.0.1...HEAD)

This version introduces breaking changes. Make sure to read on them in the Changed section below.

### Changed

- Remove auto parsing of ints for after/before params ([#5](https://github.com/mrahhal/MR.AspNetCore.Pagination/issues/5))

### Other

- Update MR.EntityFrameworkCore.KeysetPagination dependency

## 1.0.1 - 2022-02-18

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v1.0.0...v1.0.1)

- Update MR.EntityFrameworkCore.KeysetPagination dependency

## 1.0.0 - 2021-12-12

[**Full diff**](https://github.com/mrahhal/MR.EntityFrameworkCore.KeysetPagination/compare/v0.1.0...v1.0.0)

- Move `PaginationActionDetector` to main package
- Add `ConfigurePagination` for swagger and make `AddPaginationOperationFilter` obsolete
- Add efficient offset pagination for in memory list of data
- Allow overriding page size on the method call level
