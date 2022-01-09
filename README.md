# R.Systems.Auth

REST Api for users authentication and authorization. It's written in .NET 6 (C# language).

It contains the following projects:

- **R.Systems.Auth.WebApi** - ASP.NET Core Web API, .NET 6, C# language.
- **R.Systems.Auth.Infrastructure.Db** - Class library, .NET 6, C# language. It contains code responsible for communication with a database. It uses Entity Framework Core to communicate with PostgreSQL database.
- **R.Systems.Auth.Core** - Class library, .NET 6, C# language. Core functionalities of R.Systems.Auth.
- **R.Systems.Auth.FunctionalTests** - xUnit tests, .NET 6, C# language. E2E tests for endpoints available in R.Systems.Auth.WebApi.

This project is used by the web frontend written in Angular:

- [R.Systems.WebFrontend](https://github.com/lrydzkowski/R.Systems.WebFrontend)

The architecture of this solution is based on "Clean Architecture":

- [https://github.com/ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
- [https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)

Documentation:

- [https://github.com/lrydzkowski/R.Systems.Doc/wiki](https://github.com/lrydzkowski/R.Systems.Doc/wiki)
