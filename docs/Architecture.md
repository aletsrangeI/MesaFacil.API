# Arquitectura Backend

## Objetivo
Documentar la estructura y patrones de Clean Architecture aplicados en el Backend.

## Alcance
Solución Backend y sus capas.

## Cuándo consultar este documento
Para entender el flujo de ejecución, inyección de dependencias y organización de las capas.

## Información
- **Clean Architecture**:
  - El proyecto está dividido en capas: `Domain`, `UseCases` (Application), `Persistence` (Infrastructure), `Interface`, y `WebApi` (Presentation).
- **UseCases (Application)**:
  - Organizado en carpetas por funcionalidad (Feature folders: `Mesas`, `Pedidos`, `Usuarios`).
  - Utiliza un enfoque de servicios/handlers para inyectar lógica de negocio.
- **Persistence**:
  - Implementa el **Repository Pattern** y **UnitOfWork**.
  - Contexto de base de datos (`ApplicationDbContext`) configurado con Entity Framework Core.
  - Contiene interceptores para auditoría.
- **WebApi**:
  - `Scalar.AspNetCore` para la interfaz de Swagger/OpenAPI.
  - `WatchDog` implementado para el log de excepciones y monitoreo.
  - Configuración de JWT Authentication en `MesaFacil.API.Modules.Authentication`.


## Estado
> Documentado.
