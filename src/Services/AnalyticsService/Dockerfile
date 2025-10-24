FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Copy service project
COPY ["src/Services/AnalyticsService/URLShortener.AnalyticsService.csproj", "Services/AnalyticsService/"]

# Restore dependencies
RUN dotnet restore "Services/AnalyticsService/URLShortener.AnalyticsService.csproj"

# Copy everything else
COPY ["src/Shared/", "Shared/"]
COPY ["src/Services/AnalyticsService/", "Services/AnalyticsService/"]

WORKDIR "/src/Services/AnalyticsService"
RUN dotnet build "URLShortener.AnalyticsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.AnalyticsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortener.AnalyticsService.dll"]
