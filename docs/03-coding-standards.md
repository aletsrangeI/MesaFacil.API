# 03 Coding Standards

## Convenciones Generales
- **C# Version:** Se asumen características modernas de C# (.NET 9).
- **Async/Await:** Todo acceso a DB y I/O debe ser asíncrono, usando el sufijo `Async` en los métodos de repositorio e interfaces (ej. `GetByIdAsync`).
- **CancellationToken:** Pasar `CancellationToken` a todos los métodos asíncronos desde los controladores hasta los repositorios.

## Estructura por Capas
- **Namespaces:** Reflejan la estructura de carpetas (`Persistence.Repositories`, `Domain.Entities`, `MesaFacil.API.Modules.Endpoints`).
- **Entities:** Extienden de `BaseAuditableEntity` (si requieren tracking de creación/modificación) o `BaseEntity`. Los catálogos simples pueden implementar `ICatalogEntity`. Nombrados en PascalCase y singular (`Mesa`, no `Mesas`). Catálogos llevan prefijo `Cat` (`CatMoneda`).
- **Repositories:** Las interfaces se nombran `I{Entidad}Repository` y las implementaciones `{Entidad}Repository`. Se usa Scrutor para su inyección.
- **DTOs:** Separados en la capa `DTO`. Seguramente estructurados por caso de uso (`CreateMesaDto`, `MesaResponseDto`).
- **Validators:** Uso de FluentValidation. Clases terminan en `Validator` y se ubican en el proyecto `Validator`.
- **Dependency Injection:** Cada proyecto tiene un extension method (ej. `AddPersistenceServices()`) para mantener el `Program.cs` limpio.

## Naming
- Archivos y Clases: `PascalCase`.
- Interfaces: Prefijo `I` seguido de `PascalCase` (`IUnitOfWork`).
- Variables locales y parámetros: `camelCase`.
- Campos privados (si los hay): Prefijo `_` con `camelCase` (`_context`).

## Logging & Exceptions
- **WatchDog** está configurado globalmente.
- Las excepciones no controladas son atrapadas y logueadas.
- Evitar lanzar excepciones para control de flujo (preferir *Result Pattern*).
