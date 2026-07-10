# 02 Development Roadmap

Basado en el análisis de las entidades de dominio, proponemos el siguiente orden lógico de implementación de módulos.

## 1. Módulos Core (Fundamentos)
- **Seguridad y Accesos:** `Usuario`, `Rol`, `Credencial`, `AccesoRuta`. (Dependencia base para autenticar cualquier acción).
- **Configuración de Empresa:** `Empresa`, `Sucursal`, `Area`, `Formularios`. (Estructura base de la organización).
- **Catálogos Generales:** `CatMoneda`, `CatImpuesto`, `CatMetodoDePago`.

## 2. Módulos de Operación Base
- **Gestión de Espacios:** `Mesa` (depende de `Area` y `Sucursal`), `CatEstadoMesa`.
- **Gestión de Productos (Menú):** `CategoriaMenu`, `Menu`, `Producto`, `Precio`, `VarianteProducto`, `GrupoModificador`, `OpcionModificador`. (Catálogo para poder vender).
- **Cocina:** `EstacionCocina`, `CatEstacionesCocina`. (Destino de preparación de productos).

## 3. Módulos Transaccionales (Flujo de Ventas)
- **Turnos y Caja:** `Turno`, `CorteCaja`, `MovimientoCaja`. (Apertura de operaciones del día).
- **Pedidos:** `Pedido`, `PedidoAsiento`, `PedidoDetalle`, `PedidoModificador`, `EventoPedido`, `CatEstadoPedido`. (El núcleo transaccional del restaurante).

## 4. Módulos de Ejecución
- **KDS y Cocina:** `TicketCocina`, `TicketDetalle`, `CatEstadoTicketCocina`, `CatEstadoItemKDS`. (Preparación de órdenes en cocina).
- **Cuentas y Pagos:** `Cuenta`, `DetalleCuenta`, `Pago`, `DescuentoAplicado`, `Cliente`. (Liquidación de las mesas).

## Justificación
No se puede crear un *Pedido* sin antes tener *Mesas*, *Productos* y *Usuarios*. Asimismo, no se pueden realizar *Pagos* sin que exista un *Pedido* facturado en una *Cuenta*. El desarrollo debe seguir la dirección de las dependencias foráneas (Foreign Keys).
