# Etapa 1: Build y Publish con SDK de .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copiar archivos de solución y proyectos primero para optimizar la caché de capas de Docker
COPY ["MesaFacil.API.sln", "./"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Logging/Logging.csproj", "Logging/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Interface/Interface.csproj", "Interface/"]
COPY ["UseCases/UseCases.csproj", "UseCases/"]
COPY ["Validator/Validator.csproj", "Validator/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["MesaFacil.API.UnitTests/MesaFacil.API.UnitTests.csproj", "MesaFacil.API.UnitTests/"]

RUN dotnet restore "MesaFacil.API.sln"

# Copiar el código fuente completo y compilar
COPY . .
WORKDIR /src/WebApi
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime ligero (.NET 9 ASP.NET en Alpine, ~100MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Ejecutar como usuario sin privilegios por seguridad
USER $APP_UID

COPY --from=build /app/publish .

# Healthcheck interno del contenedor
HEALTHCHECK --interval=15s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/api/health || exit 1

ENTRYPOINT ["dotnet", "WebApi.dll"]
