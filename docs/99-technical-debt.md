# 99 Technical Debt

A continuación, se detalla la deuda técnica identificada a partir del estado actual del código, clasificada por prioridad.

## [CRITICAL] 
1. **Falta de Proyectos de Pruebas Unitarias/Integración:** 
   No existe rastro de una suite automatizada de pruebas para las capas de `UseCases` y `Domain`. Cualquier cambio en el sistema requiere pruebas manuales, haciéndolo frágil.

## [HIGH] 
1. **Separación Estricta en CQRS:**
   Validar si `UseCases` realmente implementa CQRS (MediatR o similares) o si son servicios god-class. El nombramiento de carpetas por entidad en UseCases (ej. `/Mesas`, `/Pedidos`) en vez de acciones explícitas (`/CreatePedido`, `/PayCuenta`) indica posible acoplamiento en servicios muy grandes.

## [MEDIUM]
1. **Auditoría Fuerte versus Event Sourcing:**
   Se utiliza `EventoPedido` para trackear el ciclo de un pedido y la clase base `BaseAuditableEntity`. En un POS con alta concurrencia, esto es aceptable, pero a futuro convendría revisar si un patrón de Event Sourcing o Domain Events (vía MediatR INotification) escalaría mejor al lanzar triggers hacia Microservicios.
2. **Uso Excesivo de Repositorios Genéricos:**
   Se usa `IGenericRepository<>` mediante reflexión para muchos catálogos. Puede ocultar lógicas de lectura complejas o necesidades de caché que más adelante se requerirán para optimización.

## [LOW]
1. **Warnings del Compilador (Ocultos):**
   Revisar si todos los campos requeridos en las entidades (como strings) que no son *nullable* están inicializados para evitar el warning `CS8618`. (Usualmente se hace con `string MiProp { get; set; } = null!;`).
