# 06 Database Conventions

## Tecnologías y ORM
- Base de datos: **PostgreSQL** a través del proveedor `Npgsql`.
- ORM: **Entity Framework Core 9**.

## Entidades y Tipos Base
- Toda tabla principal debe heredar de `BaseAuditableEntity`, el cual inyecta columnas estandarizadas:
  - `CreatedAt` (DateTime)
  - `CreatedBy` (int/string)
  - `UpdatedAt` (DateTime?)
  - `UpdatedBy` (int/string?)
  - `IsActive` (bool) -> Para borrado lógico (*soft delete*).
- Los catálogos simples implementan `ICatalogEntity` o similares y manejan descripciones o nombres fijos. Se manejan con `IGenericRepository<>`.

## Relaciones y Navegación
- Las llaves foráneas se exponen de forma explícita (ej. `public int IdSucursal { get; set; }`).
- La propiedad de navegación se define como `public Sucursal Sucursal { get; set; } = null!;` (en obligatorias).
- Se usan colecciones (`ICollection<T> = new List<T>()`) para relaciones Uno-a-Muchos.

## Migraciones
- Se almacenan en la carpeta `Migrations` del proyecto `Persistence`.
- El esquema se actualiza usando `dotnet ef migrations add <Name> --project Persistence --startup-project WebApi`.
- Al inicio de la aplicación en `Program.cs`, el sistema ejecuta `IDatabaseInitializer.InitializeAsync()` (lo que usualmente aplica las migraciones pendientes automáticamente).

## Configuraciones Especiales
- Interceptor `AuditableEntitySaveChangesInterceptor`: Inyectado en el `ApplicationDbContext`. Antes de cada guardado intercepta cambios (Insert/Update) e inyecta la fecha y el usuario actual en los campos de auditoría.
- **Soporte JSON:** Se habilitó `EnableDynamicJson()` en la configuración del DataSource de Npgsql en `ConfigureServices.cs` para el uso nativo de campos JSON/JSONB con `List<T>`.
