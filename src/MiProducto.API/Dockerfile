FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["src/MiProducto.API/MiProducto.API.csproj", "MiProducto.API/"]
COPY ["src/MiProducto.Application/MiProducto.Application.csproj", "MiProducto.Application/"]
COPY ["src/MiProducto.Domain/MiProducto.Domain.csproj", "MiProducto.Domain/"]
COPY ["src/MiProducto.Infrastructure/MiProducto.Infrastructure.csproj", "MiProducto.Infrastructure/"]

RUN dotnet restore "MiProducto.API/MiProducto.API.csproj"

COPY src/ .
RUN dotnet build "MiProducto.API/MiProducto.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiProducto.API/MiProducto.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p logs
ENTRYPOINT ["dotnet", "MiProducto.API.dll"]