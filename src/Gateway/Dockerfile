FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY ["src/Gateway/URLShortener.Gateway.csproj", "Gateway/"]
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Restore packages
RUN dotnet restore "Gateway/URLShortener.Gateway.csproj"

# Copy all source code
COPY src/Gateway/. Gateway/
COPY src/Shared/. Shared/

# Build project
WORKDIR "/src/Gateway"
RUN dotnet build "URLShortener.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Gateway"
RUN dotnet publish "URLShortener.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.Gateway.dll"]