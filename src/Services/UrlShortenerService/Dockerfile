FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Copy service project
COPY ["src/Services/UrlShortenerService/URLShortener.UrlShortenerService.csproj", "Services/UrlShortenerService/"]

# Restore dependencies
RUN dotnet restore "Services/UrlShortenerService/URLShortener.UrlShortenerService.csproj"

# Copy everything else
COPY ["src/Shared/", "Shared/"]
COPY ["src/Services/UrlShortenerService/", "Services/UrlShortenerService/"]

WORKDIR "/src/Services/UrlShortenerService"
RUN dotnet build "URLShortener.UrlShortenerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.UrlShortenerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortener.UrlShortenerService.dll"]