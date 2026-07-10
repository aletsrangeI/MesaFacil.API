# Base de Datos

## Objetivo
Documentar la estructura, relaciones y configuraciones de la base de datos.

## Alcance
Contexto de datos, migraciones y mapeo ORM.

## Cuándo consultar este documento
Al agregar entidades, modificar relaciones o planificar migraciones.

## Información
- **ORM**: Entity Framework Core.
- **Estructura**: Entidades que heredan de `BaseEntity` y `BaseAuditableEntity`. Uso de `ICatalogEntity` para catálogos.
- **Configuraciones (Fluent API)**: Centralizadas en la carpeta `Configurations` dentro de `Persistence`. No usar atributos de DataAnnotations en las entidades del dominio para configuración de base de datos.
- **Migraciones**: Gestionadas en la carpeta `Migrations`. Al agregar cambios, usar `dotnet ef migrations add ...`.
- **Auditoría**: Controlado automáticamente por interceptores (`Interceptors`) que llenan los campos de creación/modificación.


## Estado
> Documentado.
