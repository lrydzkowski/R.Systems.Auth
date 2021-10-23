# R.Systems.Auth

REST Api for users authentication and authorization. It's written in .NET 5 (C# language).

In the first version it contains 2 projects:

- R.Systems.Auth - ASP.NET Core Web API, .NET 5, C# language.
- R.Systems.Auth.Common - Class library, .NET 5, C# language. Types used by the rest of projects in R.Systems.Auth.
- R.Systems.Auth.Db - Class library, .NET 5, C# language. Library containing code responsible for communication with 
a database for R.Systems.Auth. It uses Entity Framework Core to communicate with PostgreSQL database.

This project is used by the web frontend written in Angular: 
[R.Systems.WebFrontend](https://github.com/lrydzkowski/R.Systems.WebFrontend).

**Project is under construction!**
