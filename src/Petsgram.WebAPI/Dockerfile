FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /PetsgramApp

COPY ["src/Petsgram.Domain/Petsgram.Domain.csproj", "Petsgram.Domain/"]
COPY ["src/Petsgram.Application/Petsgram.Application.csproj", "Petsgram.Application/"]
COPY ["src/Petsgram.Infrastructure/Petsgram.Infrastructure.csproj", "Petsgram.Infrastructure/"]
COPY ["src/Petsgram.WebAPI/Petsgram.WebAPI.csproj", "Petsgram.WebAPI/"]

RUN dotnet restore "Petsgram.WebAPI/Petsgram.WebAPI.csproj"

COPY ["src/Petsgram.Domain/", "Petsgram.Domain/"]
COPY ["src/Petsgram.Application/", "Petsgram.Application/"]
COPY ["src/Petsgram.Infrastructure/", "Petsgram.Infrastructure/"]
COPY ["src/Petsgram.WebAPI/", "Petsgram.WebAPI/"]

RUN dotnet publish "Petsgram.WebAPI/Petsgram.WebAPI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /PetsgramApp
COPY --from=build /PetsgramApp/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Petsgram.WebAPI.dll"]