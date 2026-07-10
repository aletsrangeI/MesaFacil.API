# Convenciones de la API

## Objetivo
Establecer las reglas de diseño para la API REST.

## Alcance
Endpoints, controladores, request/response models.

## Cuándo consultar este documento
Al crear nuevos endpoints o modificar la estructura de comunicación externa.

## Información
- **Endpoints**: 
  - Sustantivos en plural para rutas base (ej. `/api/mesas`).
  - Nombres explícitos configurados vía el transformador de OpenAPI en `Program.cs`.
- **Responses**: 
  - Uso de un patrón de respuesta estándar (ej. Result pattern).
  - Status Codes: 200 (OK), 201 (Created), 400 (Bad Request para validaciones), 401 (Unauthorized), 403 (Forbidden), 404 (Not Found).
- **Autorización**: Uso de JWT Tokens. Requiere autenticación por defecto en endpoints no marcados como `[AllowAnonymous]`. Políticas configuradas en los controladores.


## Estado
> Documentado.
