FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /DbMigrator

COPY ["src/Petsgram.Domain/Petsgram.Domain.csproj", "src/Petsgram.Domain/"]
COPY ["src/Petsgram.Application/Petsgram.Application.csproj", "src/Petsgram.Application/"]
COPY ["src/Petsgram.Infrastructure/Petsgram.Infrastructure.csproj", "src/Petsgram.Infrastructure/"]
COPY ["src/Petsgram.DbMigrator/Petsgram.DbMigrator.csproj", "src/Petsgram.DbMigrator/"]

RUN dotnet restore "src/Petsgram.DbMigrator/Petsgram.DbMigrator.csproj"

COPY ["src/Petsgram.Domain/", "src/Petsgram.Domain/"]
COPY ["src/Petsgram.Application/", "src/Petsgram.Application/"]
COPY ["src/Petsgram.Infrastructure/", "src/Petsgram.Infrastructure/"]
COPY ["src/Petsgram.DbMigrator/", "src/Petsgram.DbMigrator/"]

RUN dotnet publish "src/Petsgram.DbMigrator/Petsgram.DbMigrator.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /DbMigrator
COPY --from=build /DbMigrator/out .

ENTRYPOINT ["dotnet", "Petsgram.DbMigrator.dll"]