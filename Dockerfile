FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["./R.Systems.Auth.Core/.", "R.Systems.Auth.Core/"]
COPY ["./R.Systems.Auth.Infrastructure.Db/.", "R.Systems.Auth.Infrastructure.Db/"]
COPY ["./R.Systems.Auth.WebApi/.", "R.Systems.Auth.WebApi/"]
WORKDIR "/src/R.Systems.Auth.WebApi"
RUN dotnet restore "R.Systems.Auth.WebApi.csproj"
RUN dotnet build "R.Systems.Auth.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "R.Systems.Auth.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "R.Systems.Auth.WebApi.dll"]