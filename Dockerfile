FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/ApiRest.Domain/ApiRest.Domain.csproj             src/ApiRest.Domain/
COPY src/ApiRest.Application/ApiRest.Application.csproj   src/ApiRest.Application/
COPY src/ApiRest.Infrastructure/ApiRest.Infrastructure.csproj src/ApiRest.Infrastructure/
COPY src/ApiRest.API/ApiRest.API.csproj                   src/ApiRest.API/

RUN dotnet restore src/ApiRest.API/ApiRest.API.csproj

COPY . .

RUN dotnet publish src/ApiRest.API/ApiRest.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ApiRest.API.dll"]