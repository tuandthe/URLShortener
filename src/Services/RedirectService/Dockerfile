FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["src/Shared/URLShortener.Shared.csproj", "Shared/"]

# Copy service project
COPY ["src/Services/RedirectService/URLShortener.RedirectService.csproj", "Services/RedirectService/"]

# Restore dependencies
RUN dotnet restore "Services/RedirectService/URLShortener.RedirectService.csproj"

# Copy everything else
COPY ["src/Shared/", "Shared/"]
COPY ["src/Services/RedirectService/", "Services/RedirectService/"]

WORKDIR "/src/Services/RedirectService"
RUN dotnet build "URLShortener.RedirectService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.RedirectService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortener.RedirectService.dll"]
