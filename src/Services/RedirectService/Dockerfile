FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY ["src/Services/RedirectService/URLShortener.RedirectService.csproj", "Services/RedirectService/"]
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Restore packages
RUN dotnet restore "Services/RedirectService/URLShortener.RedirectService.csproj"

# Copy all source code
COPY src/Services/RedirectService/. Services/RedirectService/
COPY src/Shared/. Shared/

# Build project
WORKDIR "/src/Services/RedirectService"
RUN dotnet build "URLShortener.RedirectService.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Services/RedirectService"
RUN dotnet publish "URLShortener.RedirectService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "URLShortener.RedirectService.dll"]