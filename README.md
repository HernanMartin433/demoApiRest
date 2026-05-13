# MiProducto API

REST API construida con **ASP.NET Core 8**, **Clean Architecture**, **CQRS** y **PostgreSQL**.

## Stack

| Capa | Tecnología |
|---|---|
| Framework | ASP.NET Core 8 |
| Arquitectura | Clean Architecture + CQRS |
| Mediator | MediatR |
| ORM | EF Core 8 + Npgsql |
| Validación | FluentValidation |
| Docs | Swagger / OpenAPI |
| Contenedor | Docker + docker-compose |

## Estructura

```
src/
├── MiProducto.API           → Controllers, Middleware, Program.cs
├── MiProducto.Application   → CQRS Handlers, DTOs, Interfaces, Validators
├── MiProducto.Domain        → Entidades, Value Objects
└── MiProducto.Infrastructure→ EF Core, Repositorios, UnitOfWork
```

## Levantar con Docker

```bash
docker-compose up --build
```

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## Levantar en local (sin Docker)

### 1. Requisitos
- .NET 8 SDK
- PostgreSQL corriendo en localhost:5432

### 2. Configurar conexión
Editar `src/MiProducto.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MiProductoDB;Username=TU_USUARIO;Password=TU_PASS"
  }
}
```

### 3. Instalar herramientas y crear migración

```powershell
dotnet tool install --global dotnet-ef

cd src/MiProducto.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../MiProducto.API
dotnet ef database update --startup-project ../MiProducto.API
```

### 4. Correr la API

```powershell
cd src/MiProducto.API
dotnet run
```

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/products | Lista todos los productos activos |
| GET | /api/v1/products/{id} | Obtiene un producto por ID |
| POST | /api/v1/products | Crea un producto |

### Ejemplo POST

```json
{
  "name": "Producto de prueba",
  "description": "Descripción del producto",
  "price": 999.99,
  "stock": 100
}
```

## Agregar un nuevo feature (patrón CQRS)

1. Crear `Command` o `Query` en `Application/Features/{Entidad}/`
2. Crear el `Handler` correspondiente
3. Agregar `Validator` con FluentValidation si aplica
4. Exponer desde el `Controller`

## Próximos pasos sugeridos

- [ ] Autenticación JWT
- [ ] Paginación en queries
- [ ] Logging estructurado (Serilog)
- [ ] Health checks
- [ ] Tests con xUnit + Testcontainers
