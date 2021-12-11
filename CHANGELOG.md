# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## Unreleased

- Add Empty static results in KeysetPaginationResult and OffsetPaginationResult.
- Move `PaginationActionDetector` to main package.
- Add `ConfigurePagination` for swagger and make `AddPaginationOperationFilter` obsolete.
- Add efficient offset pagination for in memory list of data.
- Allow overriding page size on the method call level.
