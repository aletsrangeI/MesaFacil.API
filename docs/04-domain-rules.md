# 04 Domain Rules

Estas reglas de negocio están dictadas por el dominio (entidades) del sistema:

## Jerarquía de Organización
- Un sistema multi-inquilino / multi-franquicia está presente: Toda data transaccional suele pertener a una `Empresa` y/o `Sucursal`.
- Una `Sucursal` contiene `Areas`, y cada `Area` contiene `Mesas`. 
- Un `Pedido` pertenece a una `Sucursal` y opcionalmente a una `Mesa` (en consumo local) y puede tener asociado un `Cliente`.

## Ciclo de Vida de Pedidos
- Todo `Pedido` debe ser abierto por un usuario (`AbiertoPorUsuario`) y registra un timestamp (`AbiertoEn`).
- El `Pedido` está asociado a un estado (`CatEstadoPedido`) y a un tipo de servicio (`CatTipoPedido` - ej. Comedor, Para Llevar).
- Los pedidos se dividen en `Asientos` (personas en la mesa) y contienen `Detalles` (items pedidos).
- Al cerrarse, se registra el usuario que lo cerró (`CerradoPorUsuario`) y el momento (`CerradoEn`).

## Producción de Cocina
- Un `Pedido` genera uno o más `TicketCocina`.
- Cada ticket se asigna a una `EstacionCocina` en particular (ej. Plancha, Bebidas) y tiene un estado de preparación (`CatEstadoTicketCocina`).

## Financiero y Caja
- Las mesas y los pedidos deben pagarse a través de `Cuentas`.
- Una misma mesa o pedido puede separarse en múltiples `Cuentas`.
- Las cuentas pueden tener uno o más `Pagos`.
- Operaciones ligadas a la gestión del día deben pertenecer a un `Turno` y finalizar con un `CorteCaja`.

## Auditoría
- Prácticamente todas las entidades transaccionales heredan de `BaseAuditableEntity`, lo que significa que el sistema rastrea quién creó, quién modificó, fechas de dichas operaciones y si el registro se encuentra activo (borrado lógico).
