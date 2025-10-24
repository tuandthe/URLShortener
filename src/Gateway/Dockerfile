FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/Gateway/URLShortener.Gateway.csproj", "Gateway/"]
RUN dotnet restore "Gateway/URLShortener.Gateway.csproj"

# Copy everything else and build
COPY ["src/Gateway/", "Gateway/"]
WORKDIR "/src/Gateway"
RUN dotnet build "URLShortener.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortener.Gateway.dll"]