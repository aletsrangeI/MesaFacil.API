# Estándares de Código Backend

## Objetivo
Definir los estándares de programación y buenas prácticas de C# y .NET para el Backend.

## Alcance
Reglas de estilo, nombrado y patrones a usar en el código.

## Cuándo consultar este documento
Durante el desarrollo de funcionalidades en MesaFacil.API.

## Información
- **Convenciones**: Uso de C# 12 / .NET 9.
- **DTOs**: Todas las respuestas al cliente y peticiones deben pasar por DTOs ubicados en la capa `DTO` o dentro de los UseCases.
- **Namespaces**: Deben coincidir con la estructura de carpetas.
- **Async/Await**: Todos los accesos a I/O (BD, red) deben usar sus versiones `Async` y recibir un `CancellationToken`. No usar `.Wait()` o `.Result`.
- **Naming**: 
  - Interfaces comienzan con `I`.
  - Métodos asíncronos terminan con `Async` (la configuración de `SuppressAsyncSuffixInActionNames` está en false).
- **Exceptions y Logging**: Manejado globalmente vía Middleware/WatchDog. Evitar try-catch vacíos o que escondan la excepción real.


## Estado
> Documentado.
