# 05 API Conventions

## Endpoints
- **Rutas (Routes):** Se usan sustantivos en plural, ej. `/api/mesas`, `/api/pedidos`.
- **Nombres en Swagger/Scalar:** Se configuran usando un transformador de operaciones en `Program.cs` para mantener nombres consistentes y limpios (evita sufijos "Async").

## Controladores
- Heredan de `ControllerBase` o alguna clase base específica del proyecto.
- Los controladores dependen directamente de interfaces de `UseCases` (como CQRS o Services).
- Inyectados de manera limpia, sin lógica de negocio en el endpoint (Thin Controllers).

## Responses & Status Codes
- `200 OK`: Petición procesada correctamente (lecturas, actualizaciones).
- `201 Created`: Recurso creado exitosamente. Se suele retornar la ruta al recurso o los datos de creación.
- `204 No Content`: Petición procesada sin retornar un cuerpo (ej. borrados).
- `400 Bad Request`: Error de validación en la entrada (atrapado por FluentValidation) o regla de negocio rota.
- `401 Unauthorized`: Token ausente, expirado o inválido.
- `403 Forbidden`: El usuario está autenticado pero no tiene el rol / acceso para la ruta requerida.
- `404 Not Found`: Recurso no encontrado en base de datos.
- `500 Internal Server Error`: Fallos inesperados (capturados por Middleware / WatchDog).

## Validaciones
- **FluentValidation** valida todo DTO de entrada antes de golpear la lógica de negocio.

## Autenticación & Seguridad
- Basado en JWT (`JwtTokenService`).
- Configurado vía `JwtOptions` desde `appsettings.json`.
- Middleware `UseAuthentication()` y `UseAuthorization()` manejan la validación del Bearer Token en cada endpoint protegido.
- Uso de CORS (`policyMesaFacil`).
