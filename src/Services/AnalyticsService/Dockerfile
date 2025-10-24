FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY src/Services/AnalyticsService/URLShortener.AnalyticsService.csproj AnalyticsService/
COPY src/Shared/URLShortener.Shared.csproj Shared/

# Restore
RUN dotnet restore AnalyticsService/URLShortener.AnalyticsService.csproj

# Copy source
COPY src/Services/AnalyticsService/ AnalyticsService/
COPY src/Shared/ Shared/

# Build
WORKDIR /src/AnalyticsService
RUN dotnet build URLShortener.AnalyticsService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish URLShortener.AnalyticsService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.AnalyticsService.dll"]