# Documentación del Modelo de Base de Datos – Sistema de Gestión de Restaurantes

## Introducción

Este sistema de gestión de restaurantes está diseñado para soportar todas las operaciones críticas en la administración de restaurantes de cualquier tamaño. El modelo de datos permite:

- **Módulos del Núcleo (Core):** Administración de múltiples empresas, sucursales, áreas y mesas.
- **Seguridad y Acceso:** Control de usuarios, roles, credenciales y turnos de trabajo.
- **Menú y Precios:** Configuración dinámica de menús, categorías, productos, variantes, precios programados, impuestos, monedas y modificadores de productos.
- **Gestión de Pedidos:** Registro detallado de pedidos, asignación por asientos (comensales), modificadores aplicados y logs de auditoría de eventos de pedidos.
- **KDS (Kitchen Display System):** Coordinación y control de estados de preparación de alimentos por estaciones de cocina.
- **Cobros y Cuentas:** Generación de cuentas con cálculo automático de impuestos/cargos, aplicación de descuentos y registro de pagos con múltiples métodos.
- **Control de Caja:** Apertura y cierre de turnos, registro de movimientos manuales de efectivo y conciliación (corte de caja).
- **Clientes:** Registro de datos de contacto de clientes para consumo e historial.
- **Inventarios:** Control de insumos por almacenes, equivalencias de presentaciones, stock en tiempo real y descarga automática basada en recetas.
- **Compras y Cuentas por Pagar (CxP):** Gestión de abastecimiento de insumos, facturas de proveedores, saldo de deudas a crédito y abonos.
- **Formularios Dinámicos:** Configuración flexible de campos, etiquetas, validaciones en JSON y fuentes de datos para el frontend.

El modelo es **modular, escalable y extensible**, lo que permite adaptar el sistema a distintos escenarios de operación.

---

## Arquitectura de Catálogos Fuertemente Tipados

Para mejorar la integridad referencial y simplificar las consultas a nivel base de datos, el sistema ha migrado del antiguo modelo de catálogo dinámico general (`Catalog` / `CatalogItem`) hacia **catálogos fuertemente tipados**, representados por tablas con el prefijo `Cat`. 

Todas las entidades de catálogo implementan la interfaz común [ICatalogEntity](file:///C:/OrionSys/MesaFacil.API/Domain/Entities/ICatalogEntity.cs) y heredan de `BaseAuditableEntity`. En el backend, esto permite delegar el comportamiento CRUD de todos los catálogos a una única clase genérica: [GenericCatalogApplication](file:///C:/OrionSys/MesaFacil.API/UseCases/Common/GenericCatalogApplication.cs).

---

## Automatización para nuevas entidades

Para evitar crear manualmente toda la estructura de capas al agregar una nueva entidad, el repositorio incluye:

- [generate_entity.sh](file:///C:/OrionSys/MesaFacil.API/scripts/generate_entity.sh) para entornos Linux/macOS.
- [generate_entity.ps1](file:///C:/OrionSys/MesaFacil.API/scripts/generate_entity.ps1) para Windows (PowerShell 7+).

Ambos generadores toman como referencia la plantilla de **Catalog** e inyectan automáticamente:

- Entidades y configuraciones de EF Core en `Domain` y `Persistence`.
- DTOs, validadores y perfiles de AutoMapper.
- Interfaces y clases de aplicación en `UseCases`.
- Repositorios, unidad de trabajo y registros de inyección en `Persistence`.
- Endpoints mínimos en `WebApi`.

### Uso básico

```bash
./scripts/generate_entity.sh Menu --plural Menus
```

En Windows (PowerShell):

```powershell
pwsh ./scripts/generate_entity.ps1 -Name Menu -Plural Menus
```

---

## Esquema de Tablas por Módulos

### 1. Núcleo (Core)
* `Empresa`
* `Sucursal`
* `Area`
* `Mesa`
* `CatEstadoMesa`

### 2. Usuarios y Seguridad
* `Usuario`
* `Rol`
* `UsuarioRol`
* `Credencial`
* `CatCredencial`
* `Turno`

### 3. Menú, Precios y Modificadores
* `Menu`
* `CategoriaMenu`
* `Producto`
* `CatEstacionesCocina`
* `VarianteProducto`
* `Precio`
* `CatImpuesto`
* `CatMoneda`
* `GrupoModificador`
* `OpcionModificador`

### 4. Pedidos
* `Pedido`
* `CatTipoPedido`
* `CatEstadoPedido`
* `PedidoAsiento`
* `PedidoDetalle`
* `CatEstadoPedidoDetalle`
* `PedidoModificador`
* `EventoPedido`

### 5. KDS (Cocina)
* `EstacionCocina`
* `TicketCocina`
* `CatEstadoTicketCocina`
* `TicketDetalle`
* `CatEstadoItemKDS`

### 6. Cobro y Cuentas
* `Cuenta`
* `CatEstadoCuenta`
* `DetalleCuenta`
* `Pago`
* `CatMetodoDePago`
* `DescuentoAplicado`
* `CatTipoDescuento`

### 7. Caja
* `MovimientoCaja`
* `CatTiposDeMovimientoCaja`
* `CorteCaja`

### 8. Clientes
* `Cliente`

### 9. Inventarios
* `Almacen`
* `CatUnidadMedida`
* `Insumo`
* `RecetaDetalle`
* `CatTipoMovimientoInventario`
* `MovimientoInventario`
* `InventarioActual`
* `Proveedor`
* `PresentacionInsumo`

### 10. Compras y Cuentas por Pagar (CxP)
* `CatTipoDocumentoCompra`
* `Compra`
* `CatEstadoCompra`
* `CompraDetalle`
* `CuentaPorPagar`
* `CatEstadoCxP`
* `AbonoCxP`

### 11. Formularios Dinámicos
* `Formulario`
* `FormField`

---

## Catálogos Simples (`Cat*`)

Estas tablas constan principalmente de campos `Id` y `Descripcion`, y se administran dinámicamente mediante el servicio de catálogos genéricos.

### Ejemplo de Datos en Catálogos Comunes

#### Tabla: `CatEstadoMesa`
| Id | Descripcion |
|----|-------------|
| 1  | Libre       |
| 2  | Ocupada     |
| 3  | Reservada   |

#### Tabla: `CatCredencial`
| Id | Descripcion |
|----|-------------|
| 1  | Password    |
| 2  | PIN         |

#### Tabla: `CatEstacionesCocina`
| Id | Descripcion |
|----|-------------|
| 1  | Cocina Caliente |
| 2  | Cocina Fría   |
| 3  | Barra/Bebidas |

#### Tabla: `CatImpuesto`
| Id | Descripcion |
|----|-------------|
| 1  | IVA 16%     |
| 2  | IVA 8%      |
| 3  | Exento      |

#### Tabla: `CatMoneda`
| Id | Descripcion |
|----|-------------|
| 1  | Peso Mexicano (MXN) |
| 2  | Dólar (USD) |

#### Tabla: `CatTipoPedido`
| Id | Descripcion |
|----|-------------|
| 1  | Comer Aquí  |
| 2  | Para Llevar |
| 3  | Domicilio   |

#### Tabla: `CatEstadoPedido`
| Id | Descripcion |
|----|-------------|
| 1  | Abierto     |
| 2  | Completado  |
| 3  | Cancelado   |

#### Tabla: `CatEstadoPedidoDetalle`
| Id | Descripcion |
|----|-------------|
| 1  | Solicitado  |
| 2  | En Cocina   |
| 3  | Entregado   |

#### Tabla: `CatEstadoTicketCocina`
| Id | Descripcion |
|----|-------------|
| 1  | En Cola     |
| 2  | Preparando  |
| 3  | Listo       |

#### Tabla: `CatEstadoItemKDS`
| Id | Descripcion |
|----|-------------|
| 1  | Pendiente   |
| 2  | Cocinando   |
| 3  | Listo       |

#### Tabla: `CatEstadoCuenta`
| Id | Descripcion |
|----|-------------|
| 1  | Abierta     |
| 2  | Pagada      |
| 3  | Cancelada   |

#### Tabla: `CatMetodoDePago`
| Id | Descripcion |
|----|-------------|
| 1  | Efectivo    |
| 2  | Tarjeta de Crédito |
| 3  | Transferencia |

#### Tabla: `CatTipoDescuento`
| Id | Descripcion |
|----|-------------|
| 1  | Porcentaje  |
| 2  | Monto Fijo  |

#### Tabla: `CatTiposDeMovimientoCaja`
| Id | Descripcion |
|----|-------------|
| 1  | Ingreso     |
| 2  | Egreso      |

#### Tabla: `CatUnidadMedida`
| Id | Nombre    | Abreviacion |
|----|-----------|-------------|
| 1  | Kilogramo | kg          |
| 2  | Litro     | L           |
| 3  | Pieza     | pz          |

#### Tabla: `CatTipoMovimientoInventario`
| Id | Descripcion | Naturaleza |
|----|-------------|------------|
| 1  | Compra      | 1          |
| 2  | Venta       | -1         |
| 3  | Merma       | -1         |

#### Tabla: `CatTipoDocumentoCompra`
| Id | Descripcion |
|----|-------------|
| 1  | Factura XML |
| 2  | Nota        |

#### Tabla: `CatEstadoCompra`
| Id | Descripcion |
|----|-------------|
| 1  | Borrador    |
| 2  | Recibida    |

#### Tabla: `CatEstadoCxP`
| Id | Descripcion |
|----|-------------|
| 1  | Pendiente   |
| 2  | Pagado      |

---

## Detalles de Tablas por Módulo

## Módulo 1: Núcleo (Core)

### Tabla: `Empresa`
- **Propósito:** Representa la entidad legal o corporativa del negocio.
- **Relaciones:** Una empresa tiene múltiples sucursales y usuarios.
- **Ejemplo de datos:**
| IdEmpresa | Nombre          | Rfc          |
|-----------|-----------------|--------------|
| 1         | Restaurante XYZ | XYZ123456AA1 |

---

### Tabla: `Sucursal`
- **Propósito:** Sucursal física que pertenece a una empresa.
- **Relaciones:** FK a `Empresa`. Contiene áreas, mesas, turnos y menús.
- **Ejemplo de datos:**
| IdSucursal | IdEmpresa | Nombre          | Direccion             | ZonaHoraria    |
|------------|-----------|-----------------|-----------------------|----------------|
| 1          | 1         | Sucursal Centro | Av. Reforma 123, CDMX | America/Mexico_City |

---

### Tabla: `Area`
- **Propósito:** Subdivisiones físicas de una sucursal para organizar las mesas.
- **Relaciones:** FK a `Sucursal`.
- **Ejemplo de datos:**
| IdArea | IdSucursal | Nombre  | Orden |
|--------|------------|---------|-------|
| 1      | 1          | Terraza | 1     |

---

### Tabla: `Mesa`
- **Propósito:** Identificación física de las mesas del restaurante.
- **Relaciones:** FK a `Sucursal`, `Area` y `CatEstadoMesa`.
- **Ejemplo de datos:**
| IdMesa | IdSucursal | IdArea | Codigo | Asientos | IdEstadoMesa |
|--------|------------|--------|--------|----------|--------------|
| 1      | 1          | 1      | M01    | 4        | 1            |

---

## Módulo 2: Usuarios y Seguridad

### Tabla: `Usuario`
- **Propósito:** Registro de los empleados y administradores del sistema.
- **Relaciones:** FK a `Empresa`.
- **Ejemplo de datos:**
| IdUsuario | IdEmpresa | NombreCompleto | User      |
|-----------|-----------|----------------|-----------|
| 1         | 1         | Juan Pérez     | jperez    |

---

### Tabla: `Rol`
- **Propósito:** Define los roles de acceso en el sistema.
- **Ejemplo de datos:**
| IdRol | Nombre | IsSystem | IsAssignable |
|-------|--------|----------|--------------|
| 1     | Admin  | 1        | 0            |
| 2     | Mesero | 0        | 1            |

---

### Tabla: `UsuarioRol`
- **Propósito:** Relación intermedia N:N entre usuarios y roles.
- **Campos:** `IdUsuario`, `IdRol`.

---

### Tabla: `Credencial`
- **Propósito:** Almacena los hashes y sales de seguridad del usuario.
- **Relaciones:** FK a `Usuario` y `CatCredencial`.
- **Ejemplo de datos:**
| IdUsuario | IdCredencial | Hash          | Salt         |
|-----------|--------------|---------------|--------------|
| 1         | 1            | a8f9cd3e...   | x29f0e3...   |

---

### Tabla: `Turno`
- **Propósito:** Controla la apertura, el arqueo de efectivo y el cierre de cajas por usuario y sucursal.
- **Relaciones:** FK a `Usuario` y `Sucursal`.
- **Ejemplo de datos:**
| IdTurno | IdUsuario | IdSucursal | Apertura            | Cierre              | CajaInicial | CajaFinal |
|---------|-----------|------------|---------------------|---------------------|-------------|-----------|
| 1       | 1         | 1          | 2026-07-09 08:00:00 | 2026-07-09 16:00:00 | 1000.00     | 4500.00   |

---

## Módulo 3: Menú, Precios y Modificadores

### Tabla: `Menu`
- **Propósito:** Encabezado del menú asignado a una sucursal.
- **Relaciones:** FK a `Sucursal`.

---

### Tabla: `CategoriaMenu`
- **Propósito:** Categorías o secciones que dividen el menú.
- **Relaciones:** FK a `Menu`.
- **Ejemplo de datos:**
| IdCategoria | IdMenu | Nombre   | Orden |
|-------------|--------|----------|-------|
| 1           | 1      | Entradas | 1     |

---

### Tabla: `Producto`
- **Propósito:** Artículos finales que se ofrecen a la venta.
- **Relaciones:** FK a `Menu`, `CategoriaMenu` y `CatEstacionesCocina`.
- **Ejemplo de datos:**
| IdProducto | IdMenu | IdCategoria | Codigo | Nombre       | Descripcion               | Activo | IdEstacionCocina |
|------------|--------|-------------|--------|--------------|---------------------------|--------|------------------|
| 1          | 1      | 1           | TACO01 | Tacos Pastor | Orden de 3 tacos al pastor| 1      | 1                |

---

### Tabla: `VarianteProducto`
- **Propósito:** Diferentes presentaciones o tamaños de un mismo producto.
- **Relaciones:** FK a `Producto`.
- **Ejemplo de datos:**
| IdVariante | IdProducto | Nombre  | Codigo  | EsDefault |
|------------|------------|---------|---------|-----------|
| 1          | 1          | Grande  | TACG    | 0         |
| 2          | 1          | Regular | TACR    | 1         |

---

### Tabla: `Precio`
- **Propósito:** Controla los precios históricos o dinámicos asignados a variantes de productos.
- **Relaciones:** FK a `VarianteProducto`, `CatImpuesto` y `CatMoneda`.
- **Ejemplo de datos:**
| IdPrecio | IdVariante | Monto  | Moneda | IdImpuesto | IdMoneda | ValidoDesde | ValidoHasta | Dias    | Horario |
|----------|------------|--------|--------|------------|----------|-------------|-------------|---------|---------|
| 1        | 1          | 120.00 | MXN    | 1          | 1        | 2026-01-01  | null        | Lu-Ma-Mi| null    |

---

### Tabla: `GrupoModificador`
- **Propósito:** Agrupa modificadores para un platillo (ej. Términos de carne, adiciones).
- **Relaciones:** FK a `Producto`.
- **Ejemplo de datos:**
| IdGrupo | IdProducto | Nombre  | MinSeleccion | MaxSeleccion | Obligatorio |
|---------|------------|---------|--------------|--------------|-------------|
| 1       | 1          | Salsas  | 0            | 2            | 0           |

---

### Tabla: `OpcionModificador`
- **Propósito:** Opciones individuales que pertenecen a un grupo modificador.
- **Relaciones:** FK a `GrupoModificador`.
- **Ejemplo de datos:**
| IdOpcion | IdGrupo | Nombre      | PrecioExtra | EsDefault |
|----------|---------|-------------|-------------|-----------|
| 1        | 1       | Salsa Verde | 0.00        | 1         |
| 2        | 1       | Guacamole   | 15.00       | 0         |

---

## Módulo 4: Pedidos

### Tabla: `Pedido`
- **Propósito:** Registra un pedido de mesa o consumo general.
- **Relaciones:** FK a `Empresa`, `Sucursal`, `Mesa`, `Cliente`, `Usuario` (abierto por / cerrado por), `CatTipoPedido` y `CatEstadoPedido`.
- **Ejemplo de datos:**
| IdPedido | IdEmpresa | IdSucursal | IdMesa | IdCliente | Personas | AbiertoPor | CerradoPor | IdTipoPedido | IdEstadoPedido | CargoServicioPct |
|----------|-----------|------------|--------|-----------|----------|------------|------------|--------------|----------------|------------------|
| 101      | 1         | 1          | 1      | null      | 2        | 1          | null       | 1            | 1              | 10.00            |

---

### Tabla: `PedidoAsiento`
- **Propósito:** Divide las comandas por comensal (asiento) para permitir cobro separado.
- **Relaciones:** FK a `Pedido`.

---

### Tabla: `PedidoDetalle`
- **Propósito:** Almacena los ítems ordenados dentro del pedido.
- **Relaciones:** FK a `Pedido`, `PedidoAsiento`, `Producto`, `VarianteProducto`, `CatImpuesto` y `CatEstadoPedidoDetalle`.
- **Ejemplo de datos:**
| IdDetalle | IdPedido | IdAsiento | IdProducto | IdVariante | ProductoNombre | VarianteNombre | Cantidad | PrecioUnitario | IdImpuesto | TasaImpuesto | MontoImpuesto | IdEstadoPedidoDetalle | Cancelado |
|-----------|----------|-----------|------------|------------|----------------|----------------|----------|----------------|------------|--------------|---------------|-----------------------|-----------|
| 1         | 101      | null      | 1          | 2          | Tacos Pastor   | Regular        | 2.00     | 120.00         | 1          | 16.00        | 38.40         | 1                     | 0         |

---

### Tabla: `PedidoModificador`
- **Propósito:** Opciones extras aplicadas a un ítem ordenado (ej: Tacos con salsa extra).
- **Relaciones:** FK a `PedidoDetalle` y `OpcionModificador`.
- **Ejemplo de datos:**
| IdModificador | IdDetalle | IdOpcion | OpcionNombre | PrecioExtra |
|---------------|-----------|----------|--------------|-------------|
| 1             | 1         | 2        | Guacamole    | 15.00       |

---

### Tabla: `EventoPedido`
- **Propósito:** Bitácora de cambios y logs de auditoría sobre el flujo de un pedido (ej: cancelaciones, cambios de mesa).
- **Relaciones:** FK a `Pedido` y `Usuario`.

---

## Módulo 5: KDS (Cocina)

### Tabla: `EstacionCocina`
- **Propósito:** Registra las áreas de producción física (Cocina Caliente, Fría, etc.).
- **Relaciones:** FK a `Sucursal` y `CatEstacionesCocina`.
- **Ejemplo de datos:**
| IdEstacion | IdSucursal | IdCatEstacionCocina | Nombre             | Activo |
|------------|------------|---------------------|--------------------|--------|
| 1          | 1          | 1                   | Cocina Principal   | 1      |

---

### Tabla: `TicketCocina`
- **Propósito:** El ticket o comanda digital enviada a la estación de cocina.
- **Relaciones:** FK a `EstacionCocina`, `Pedido` y `CatEstadoTicketCocina`.
- **Ejemplo de datos:**
| IdTicket | IdEstacion | IdPedido | IdEstadoTicketCocina | CreadoEn            | CompletadoEn |
|----------|------------|----------|----------------------|---------------------|--------------|
| 1        | 1          | 101      | 1                    | 2026-07-09 12:10:00 | null         |

---

### Tabla: `TicketDetalle`
- **Propósito:** Artículos individuales dentro del ticket de cocina.
- **Relaciones:** FK a `TicketCocina`, `PedidoDetalle` y `CatEstadoItemKDS`.

---

## Módulo 6: Cobro y Cuentas

### Tabla: `Cuenta`
- **Propósito:** Agrupa los cargos finales y totales de un pedido para su facturación y cobro.
- **Relaciones:** FK a `Pedido` y `CatEstadoCuenta`.
- **Ejemplo de datos:**
| IdCuenta | IdPedido | Subtotal | DescuentoTotal | CargoServicio | ImpuestoTotal | Total  | IdEstadoCuenta |
|----------|----------|----------|----------------|---------------|---------------|--------|----------------|
| 1        | 101      | 240.00   | 0.00           | 24.00         | 38.40         | 302.40 | 1              |

---

### Tabla: `DetalleCuenta`
- **Propósito:** Desglose individual de los cargos asignados a la cuenta.
- **Relaciones:** FK a `Cuenta` y `PedidoDetalle`.

---

### Tabla: `Pago`
- **Propósito:** Historial de transacciones de pago aplicadas a una cuenta.
- **Relaciones:** FK a `Cuenta`, `Usuario` (recibido por) y `CatMetodoDePago`.
- **Ejemplo de datos:**
| IdPago | IdCuenta | Monto  | Moneda | Propina | PagadoEn            | Referencia | RecibidoPor | IdMetodoDePago |
|--------|----------|--------|--------|---------|---------------------|------------|-------------|----------------|
| 1      | 1        | 302.40 | MXN    | 30.00   | 2026-07-09 12:35:00 | null       | 1           | 1              |

---

### Tabla: `DescuentoAplicado`
- **Propósito:** Detalla promociones o cortesías aplicadas sobre una cuenta.
- **Relaciones:** FK a `Cuenta` y `CatTipoDescuento`.

---

## Módulo 7: Caja

### Tabla: `MovimientoCaja`
- **Propósito:** Entradas y salidas de efectivo (ej: retiro parcial, pago menor).
- **Relaciones:** FK a `Turno` y `CatTiposDeMovimientoCaja`.
- **Ejemplo de datos:**
| IdMovimiento | IdTurno | IdTipoMovimientoCaja | Monto  | Nota                      |
|--------------|---------|----------------------|--------|---------------------------|
| 1            | 1       | 2                    | 200.00 | Pago de insumos de verdura|

---

### Tabla: `CorteCaja`
- **Propósito:** Reporte de conciliación final de caja.
- **Relaciones:** FK a `Turno`, `Sucursal` y `Usuario` (creado por).
- **Ejemplo de datos:**
| IdCorteCaja | IdTurno | IdSucursal | TotalVentas | TotalPagos | TotalEfectivo | TotalTarjeta | CajaEsperada | Declarado | Diferencia |
|-------------|---------|------------|-------------|------------|---------------|--------------|--------------|-----------|------------|
| 1           | 1       | 1          | 4500.00     | 4500.00    | 3500.00       | 1000.00      | 4500.00      | 4500.00   | 0.00       |

---

## Módulo 8: Clientes

### Tabla: `Cliente`
- **Propósito:** Directorio de clientes para servicios de domicilio, facturación y lealtad.
- **Relaciones:** FK a `Empresa`.
- **Ejemplo de datos:**
| IdCliente | IdEmpresa | Nombre       | Telefono   | Correo            |
|-----------|-----------|--------------|------------|-------------------|
| 1         | 1         | Carlos López | 5551234567 | clopez@domain.com |

---

## Módulo 9: Inventarios

### Tabla: `Almacen`
- **Propósito:** Ubicaciones de stock físico en el restaurante (ej: Bodega Principal, Barra).
- **Relaciones:** FK a `Sucursal`.
- **Ejemplo de datos:**
| IdAlmacen | IdSucursal | Nombre            | Activo |
|-----------|------------|-------------------|--------|
| 1         | 1          | Bodega Principal  | 1      |
| 2         | 1          | Barra de Bebidas  | 1      |

---

### Tabla: `Insumo`
- **Propósito:** Ingredientes o insumos base que se controlan en stock.
- **Relaciones:** FK a `CatUnidadMedida`.
- **Ejemplo de datos:**
| IdInsumo | Nombre               | IdUnidadMedidaBase | CostoPromedio | EsInventariable |
|----------|----------------------|--------------------|---------------|-----------------|
| 1        | Carne de Res (Gramos)| 4 (g)              | 0.1500        | 1               |
| 2        | Tortillas (Piezas)   | 3 (pz)             | 0.5000        | 1               |

---

### Tabla: `RecetaDetalle`
- **Propósito:** Receta/Fórmula para descontar automáticamente los insumos al vender un platillo o variante.
- **Relaciones:** FK a `Producto`, `VarianteProducto` e `Insumo`.
- **Ejemplo de datos:**
| IdRecetaDetalle | IdProducto | IdVariante | IdInsumo | Cantidad |
|-----------------|------------|------------|----------|----------|
| 1               | 1          | 2 (Regular)| 1 (Carne)| 150.0000 |
| 2               | 1          | 2 (Regular)| 2 (Torti)| 3.0000   |

---

### Tabla: `MovimientoInventario`
- **Propósito:** Registra todas las transacciones físicas de inventario (Merma, Venta, Compra, Ajuste).
- **Relaciones:** FK a `Almacen`, `Insumo`, `CatTipoMovimientoInventario` y `Usuario`.
- **Ejemplo de datos:**
| IdMovimiento | IdAlmacen | IdInsumo | IdTipoMovimiento | Cantidad | CostoUnitario | FechaMovimiento      | IdUsuario | Notas                  | ReferenciaOrigen |
|--------------|-----------|----------|------------------|----------|---------------|----------------------|-----------|------------------------|------------------|
| 1            | 1         | 1        | 2 (Venta)        | -150.00  | 0.15          | 2026-07-09 12:35:00  | 1         | Descontado por Pedido  | Detalle: 1       |

---

### Tabla: `InventarioActual`
- **Propósito:** Mantiene la cantidad en stock de insumos por almacén en tiempo real.
- **Relaciones:** PK Compuesta de `IdAlmacen` (FK) e `IdInsumo` (FK).
- **Ejemplo de datos:**
| IdAlmacen | IdInsumo | CantidadActual | UltimaActualizacion |
|-----------|----------|----------------|---------------------|
| 1         | 1        | 24850.0000     | 2026-07-09 12:35:00 |

---

### Tabla: `Proveedor`
- **Propósito:** Proveedores del restaurante.
- **Relaciones:** FK a `Empresa`.
- **Ejemplo de datos:**
| IdProveedor | IdEmpresa | Nombre              | Rfc          | DiasCredito | Activo |
|-------------|-----------|---------------------|--------------|-------------|--------|
| 1           | 1         | Proveedora de Carne | CAR991231AA1 | 15          | 1      |

---

### Tabla: `PresentacionInsumo`
- **Propósito:** Unidades y empaques de compra (ej: caja de tortillas, bolsa de carne).
- **Relaciones:** FK a `Insumo`.
- **Ejemplo de datos:**
| IdPresentacion | IdInsumo | Nombre          | FactorConversion |
|----------------|----------|-----------------|------------------|
| 1              | 2 (Tort) | Paquete 100 pz  | 100.0000         |

---

## Módulo 10: Compras y Cuentas por Pagar (CxP)

### Tabla: `Compra`
- **Propósito:** Cabecera de compras de insumos para abastecer almacenes.
- **Relaciones:** FK a `Sucursal`, `Proveedor`, `CatTipoDocumentoCompra`, `Usuario` y `CatEstadoCompra`.
- **Ejemplo de datos:**
| IdCompra | IdSucursal | IdProveedor | IdTipoDocumentoCompra | Folio | FechaCompra | Subtotal | Impuestos | Total   | IdUsuarioRegistro | IdEstadoCompra |
|----------|------------|-------------|-----------------------|-------|-------------|----------|-----------|---------|-------------------|----------------|
| 1        | 1          | 1           | 1                     | F-891 | 2026-07-09  | 1000.00  | 160.00    | 1160.00 | 1                 | 2 (Recibida)   |

---

### Tabla: `CompraDetalle`
- **Propósito:** Desglose de insumos adquiridos por compra, calculando conversión a unidad base.
- **Relaciones:** FK a `Compra`, `Insumo` y `PresentacionInsumo`.
- **Ejemplo de datos:**
| IdCompraDetalle | IdCompra | IdInsumo | IdPresentacion | CantidadComprada | CostoUnitarioCompra | CantidadBaseRecibida | CostoUnitarioBase |
|-----------------|----------|----------|----------------|------------------|---------------------|----------------------|-------------------|
| 1               | 1        | 2        | 1 (Paq 100pz)  | 10.0000          | 50.0000             | 1000.0000            | 0.5000            |

---

### Tabla: `CuentaPorPagar`
- **Propósito:** Deudas pendientes de pago a proveedores derivadas de compras a crédito.
- **Relaciones:** FK a `Compra` (Unique), `Proveedor` y `CatEstadoCxP`.
- **Ejemplo de datos:**
| IdCuentaPorPagar | IdCompra | IdProveedor | MontoOriginal | SaldoPendiente | FechaEmision | FechaVencimiento | IdEstadoCxP |
|------------------|----------|-------------|---------------|----------------|--------------|------------------|-------------|
| 1                | 1        | 1           | 1160.00       | 1160.00        | 2026-07-09   | 2026-07-24       | 1           |

---

### Tabla: `AbonoCxP`
- **Propósito:** Amortizaciones y abonos realizados sobre la cuenta por pagar.
- **Relaciones:** FK a `CuentaPorPagar`, `CatMetodoDePago`, `MovimientoCaja`, `Usuario`.
- **Ejemplo de datos:**
| IdAbono | IdCuentaPorPagar | Monto  | FechaAbono          | IdMetodoDePago | Referencia | IdMovimientoCaja | IdUsuario |
|---------|------------------|--------|---------------------|----------------|------------|------------------|-----------|
| 1       | 1                | 500.00 | 2026-07-10 10:00:00 | 1              | N/A        | null             | 1         |

---

## Módulo 11: Formularios Dinámicos

### Tabla: `Formulario`
- **Propósito:** Configura los formularios dinámicos del sistema de forma centralizada.
- **Ejemplo de datos:**
| IdFormulario | Codigo | Nombre              | Descripcion                     | IsActive |
|--------------|--------|---------------------|---------------------------------|----------|
| 1            | LOGIN  | Pantalla de Login   | Campos requeridos para el login | 1        |

---

### Tabla: `FormField`
- **Propósito:** Define los campos y validaciones de un formulario dinámico.
- **Relaciones:** FK a `Formulario`.
- **Ejemplo de datos:**
| IdFormField | IdFormulario | Type  | Name     | Placeholder | Label    | Value | ValidationsJson           | OptionsJson | DataSource | Orden | IsActive |
|-------------|--------------|-------|----------|-------------|----------|-------|---------------------------|-------------|------------|-------|----------|
| 1           | 1            | email | username | ej@mail.com | Usuario  | null  | `[{"type": "required"}]`  | null        | null       | 1     | 1        |
| 2           | 1            | pass  | password | ********    | Password | null  | `[{"type": "required"}]`  | null        | null       | 2     | 1        |

---

## Flujos Operativos del Sistema

### 1. Ciclo de Venta (Mesa -> KDS -> Caja)
1. **Apertura de Turno:** El cajero inicia un `Turno` en una `Sucursal` declarando una `CajaInicial`.
2. **Registro de Pedido:** Un usuario abre un `Pedido` asignando una `Mesa`. Se asocian comensales mediante `PedidoAsiento` e ítems en `PedidoDetalle`.
3. **Producción (KDS):** Si un producto del detalle pertenece a una estación de preparación, se genera un `TicketCocina` y se asigna su estado (`CatEstadoTicketCocina`). El KDS actualiza el avance de preparación (`CatEstadoItemKDS`).
4. **Cierre de Consumo:** Se crea una `Cuenta` ligada al pedido. Se aplican promociones (`DescuentoAplicado`) y se calcula el total con impuestos.
5. **Transacción y Cierre:** El cajero registra uno o más pagos (`Pago`). Al completarse el saldo, la `Cuenta` y el `Pedido` cambian a estado "Pagado / Cerrado".

### 2. Ciclo de Control de Inventario y Recetas
1. **Configuración de Receta:** Se asocia una variante de producto con insumos y cantidades en `RecetaDetalle`.
2. **Descargo Automático:** Al pagar o confirmar el pedido, el sistema dispara el descargo automático de stock en `InventarioActual` restando la cantidad calculada en la receta y registrando un `MovimientoInventario`.
3. **Conciliación:** Se realizan inventarios físicos periódicos y se registran ajustes mediante movimientos manuales.

### 3. Ciclo de Abastecimiento (Compras y Cuentas por Pagar)
1. **Registro de Compra:** Se ingresa una `Compra` con sus respectivas líneas de `CompraDetalle` asociadas a un `Proveedor`.
2. **Ingreso a Almacén:** Al marcar la compra como "Recibida", se calcula la conversión a unidades base (`CantidadBaseRecibida`) e incrementa el stock en `InventarioActual` por medio de un movimiento tipo Compra.
3. **Financiamiento:** Si la compra es a crédito, se genera una `CuentaPorPagar` asignando la fecha de vencimiento. El saldo se va liquidando registrando amortizaciones (`AbonoCxP`).

### 4. Ciclo de Formularios Dinámicos
1. **Renderizado Dinámico:** El frontend solicita al backend la configuración del `Formulario` por su código identificador.
2. **Generación de Inputs:** El sistema recupera los `FormField` asociados, renderizando de manera dinámica los controles, validaciones e inputs en base al JSON configurado.

---

## Conclusión

El modelo de datos cubre de manera integral la operación y administración financiera de un restaurante moderno. Los catálogos simplificados bajo el esquema genérico optimizan la modularidad del código, mientras que los flujos de inventarios y cuentas por pagar proveen un control robusto de los costos operativos.
