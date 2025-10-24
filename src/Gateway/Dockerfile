FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj - QUAN TRỌNG: Path relative to context (src/Gateway)
COPY ["URLShortener.Gateway.csproj", "./"]
COPY ["../Shared/URLShortener.Shared.csproj", "../Shared/"]

# Restore
RUN dotnet restore "URLShortener.Gateway.csproj"

# Copy source
COPY . .

# Build
RUN dotnet build "URLShortener.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway injects PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.Gateway.dll"]