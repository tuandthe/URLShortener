FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj
COPY ["URLShortener.UrlShortenerService.csproj", "./"]
COPY ["../../Shared/URLShortener.Shared.csproj", "../../Shared/"]

# Restore
RUN dotnet restore "URLShortener.UrlShortenerService.csproj"

# Copy source
COPY . .

# Build
RUN dotnet build "URLShortener.UrlShortenerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.UrlShortenerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.UrlShortenerService.dll"]