FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY ["src/Services/AnalyticsService/URLShortener.AnalyticsService.csproj", "Services/AnalyticsService/"]
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Restore packages
RUN dotnet restore "Services/AnalyticsService/URLShortener.AnalyticsService.csproj"

# Copy all source code
COPY src/Services/AnalyticsService/. Services/AnalyticsService/
COPY src/Shared/. Shared/

# Build project
WORKDIR "/src/Services/AnalyticsService"
RUN dotnet build "URLShortener.AnalyticsService.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Services/AnalyticsService"
RUN dotnet publish "URLShortener.AnalyticsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.AnalyticsService.dll"]