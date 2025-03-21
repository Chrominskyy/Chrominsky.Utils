# Changelog

All notable changes to this project will be documented in this file.

This format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.2] - 2025-03-13
### Added
- `DatabaseColumnTypes` - added enum class to support handling different database column types for similar data types

## [1.2.1] - 2025-03-12
### Changed
- `BaseDatabaseRepository` - fixed `GetTableColumnsAsync` method to allow tableName to be passed as parameter

## [1.2.0] - 2025-03-12
### Added
- `TableColumns` - added model to support table columns
- `TableColumnsMapper` - added mapper to map table columns to model
- `TableColumnsDto` - added dto to support table columns

### Changed
- `BaseDatabaseRepository` - added method `GetTableColumnsAsync` to retrieve table columns
- `IBaseDatabaseRepository` - added method `GetTableColumnsAsync` to retrieve table columns

## [1.1.3] - 2024-12-06
### Changed
- `BaseDatabaseEntity` - fixed library for json serialization
- `BaseEntity` - fixed library for json serialization
- `BaseDatabaseRepository` - fixed Search method + added method `GetAllActiveAsync` to retrieve only active entities
- `IBaseDatabaseRepository` - added method `GetAllActiveAsync` to retrieve only active entities
- `SearchParameterRequest` - added `IncludeNotActive` parameter to support searching for not active entities

## [1.1.2] - 2024-12-05
### Changed
- tag fix for nuget

## [1.1.1] - 2024-12-05
### Changed
- `DatabaseEntityStatus` enum is now being parsed to string + summary change
- `BaseDatabaseEntity` marked Id column Key
- `BaseDatabaseRepository` fixed updating entity

## [1.1.0] - 2024-08-27
### Added
- `SimpleEmailSender` - to support email sending with very few commands.

## [1.0.9] - 2024-06-13
### Fixed
- `ObjectVersioningRepository` - fixed `UpdatedOn` mapping
- `BaseDatabaseReposiotry` - fixed `UpdatedBy` mapping for ObjectVersioningRepository

## [1.0.8] - 2024-06-12
### Added
- `ObjectVersion` - added model to support tracing object changes
- `ObjectVersioningRepository`
- `IObjectVersioningRepository`

## [1.0.7] - 2024-06-11
### Changed
- `IBaseDatabaseRepository` - added `GetPaginatedAsync` method to support paginated Get
- `BaseDatabaseRepository` - added implementation of `GetPaginatedAsync` method

### Added
- `PaginatedRequest` - added request supporting pagination
- `PaginatedResponse` - added response supporting pagination

## [1.0.6] - 2024-06-10
### Fixed
- `BaseDatabaseRepository` - fixed UpdatedAt not updating on `UpdateAsync` method

### Added
- `SearchParameterRequest` - added request structure for `SearchAsync` method in `IBaseDatabaseRepository`
- `SearchParameter` - added parameter structure
- `SearchOperator` - added search operator enum
- `SearchOrder` - added search order enum
- `ErrorMessage` - added error message structure
- Unit tests project `Chrominsky.Utils.Tests` + few unit tests

### Changed
- `IBaseDatabaseRepository` - added `SearchAsync` method for supporting search functionality
- `BaseDatabaseRepository` - added implementation of `SearchAsync` method

## [1.0.5] - 2024-06-05
### Changed
- `BaseDatabaseRepository` - changed UpdateAsync to update only changed values leaving old ones intact 

## [1.0.4] - 2024-06-04
### Changed
- `IBaseDatabaseRepository` - added method GetAllAsync to retrieve all entities from db
- `BaseDatabaseRepository` - added implementation of GetAllAsync method

## [1.0.3] - 2024-06-03
### Added
- `BaseSimpleEntity`
- `DatabaseEntityStatus`
- `IBaseDatabaseEntity`
- `IBaseDatabaseEntityWithTenantId`
- `BaseDatabaseEntityWithTenantId`

### Changed
- `BaseDatabaseEntity` - now implements `IBaseDatabaseEntity`
- `BaseDatabaseRepository` - now uses `DatabaseEntityStatus` enum for deletion
- `IBaseDatabaseRepository` - T needs to implement `IBaseDatabaseEntity`
- `BaseEntity` - now uses `DatabaseEntityStatus`

## [1.0.2] - 2024-05-30
### Fixed
- Fixed readme for nuget packages

## [1.0.1] - 2024-05-30
### Added
- Added changelog

## [1.0.0] - 2024-05-30
### Added
- Created project