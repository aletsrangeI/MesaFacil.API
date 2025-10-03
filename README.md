# Documentación del Modelo de Base de Datos – Sistema de Gestión de Restaurantes

## Introducción

Este sistema de gestión de restaurantes está diseñado para soportar todas las operaciones críticas en la administración de restaurantes de cualquier tamaño. El modelo de datos permite:

- Manejo centralizado de catálogos para estados, tipos de pedidos, métodos de pago, impuestos y configuraciones.
- Administración de múltiples empresas, sucursales, áreas y mesas.
- Control de usuarios, roles y credenciales de acceso.
- Configuración dinámica de menús, variantes de productos y modificadores.
- Registro detallado de pedidos y seguimiento de estados.
- Coordinación de estaciones de cocina (KDS).
- Generación de cuentas, pagos, descuentos y cortes de caja.
- Gestión de clientes y su historial de consumo.

El modelo es **modular, escalable y extensible**, lo que permite adaptar el sistema a distintos escenarios de operación.

---

## Automatización para nuevas entidades

Para evitar crear manualmente toda la estructura de capas al agregar una nueva entidad, el repositorio incluye:

- `scripts/generate_entity.sh` para entornos Linux/macOS.
- `scripts/generate_entity.ps1` para Windows (PowerShell 7+).

Ambos generadores toman como referencia la implementación de **Catalog** y actualizan automáticamente:

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

El parámetro de nombre debe estar en PascalCase. Si el plural no sigue la forma estándar agregando `s`, proporciónalo con `--plural` (`-Plural` en PowerShell). Para validar los archivos que se crearían sin escribirlos utiliza `--dry-run` o `-DryRun`:

```bash
./scripts/generate_entity.sh Menu --dry-run
```

```powershell
pwsh ./scripts/generate_entity.ps1 -Name Menu -DryRun
```

Los scripts detendrán la ejecución si alguno de los archivos de destino ya existe.

## Esquema de Tablas por Módulos

1. **Catálogos**
   - Catalog
   - CatalogItem

2. **Núcleo**
   - Empresa
   - Sucursal
   - Area
   - Mesa

3. **Usuarios y Seguridad**
   - Usuario
   - Rol
   - UsuarioRol
   - Credencial
   - Turno

4. **Menú / Precios / Modificadores**
   - Menu
   - CategoriaMenu
   - Producto
   - VarianteProducto
   - Precio
   - GrupoModificador
   - OpcionModificador

5. **Pedidos**
   - Pedido
   - PedidoAsiento
   - PedidoDetalle
   - PedidoModificador
   - EventoPedido

6. **KDS (Cocina)**
   - EstacionCocina
   - TicketCocina
   - TicketDetalle

7. **Cobro**
   - Cuenta
   - DetalleCuenta
   - Pago
   - DescuentoAplicado

8. **Caja**
   - MovimientoCaja
   - CorteCaja

9. **Clientes**
   - Cliente

---

## Catálogos

### Tabla: `Catalog`
- **Propósito:** Define los catálogos maestros (ejemplo: `EstadoPedido`, `MetodoPago`).
- **Relaciones:** Cada `Catalog` se asocia a múltiples `CatalogItem`.
- **Campos clave:**  
  - `Code`: Identificador natural del catálogo.  
  - `IsActive`: Activo/inactivo.  
- **Ejemplo de datos:**

| Id | Code          | Name               | IsActive |
|----|---------------|--------------------|----------|
| 1  | EstadoPedido  | Estados de Pedido  | 1        |
| 2  | MetodoPago    | Métodos de Pago    | 1        |

---

### Tabla: `CatalogItem`
- **Propósito:** Define los ítems de cada catálogo.  
- **Relaciones:** FK hacia `Catalog`. Referenciado en múltiples tablas.  
- **Campos clave:**  
  - `Code`: Clave interna (`ABIERTO`, `CERRADO`).  
  - `SortOrder`: Orden de despliegue.  
- **Ejemplo de datos:**

| Id | CatalogId | Code     | Name      | SortOrder | IsActive |
|----|-----------|----------|-----------|-----------|----------|
| 1  | 1         | ABIERTO  | Abierto   | 1         | 1        |
| 2  | 1         | CERRADO  | Cerrado   | 2         | 1        |
| 3  | 2         | EFECTIVO | Efectivo  | 1         | 1        |

---

## Núcleo

### Tabla: `Empresa`
- **Propósito:** Representa la entidad corporativa.  
- **Relaciones:** Una empresa tiene múltiples `Sucursal` y `Usuario`.  
- **Ejemplo de datos:**

| IdEmpresa | Nombre          | Rfc          |
|-----------|-----------------|--------------|
| 1         | Restaurante XYZ | XYZ123456AA1 |

---

### Tabla: `Sucursal`
- **Propósito:** Representa una sucursal de una empresa.  
- **Relaciones:** FK a `Empresa`, contiene `Area`, `Mesa`, `Menu`, `Turno`.  
- **Ejemplo de datos:**

| IdSucursal | IdEmpresa | Nombre            | Direccion                |
|------------|-----------|-------------------|--------------------------|
| 1          | 1         | Sucursal Centro   | Av. Reforma 123, CDMX    |

---

### Tabla: `Area`
- **Propósito:** Divide la sucursal en áreas (ej. terraza, interior).  
- **Relaciones:** FK a `Sucursal`. Usada por `Mesa`.  
- **Ejemplo de datos:**

| IdArea | IdSucursal | Nombre   | Orden |
|--------|------------|----------|-------|
| 1      | 1          | Terraza  | 1     |

---

### Tabla: `Mesa`
- **Propósito:** Identifica cada mesa del restaurante.  
- **Relaciones:** FK a `Sucursal`, `Area`, catálogos de estado.  
- **Ejemplo de datos:**

| IdMesa | IdSucursal | Codigo | Asientos | EstadoItemId |
|--------|------------|--------|----------|--------------|
| 1      | 1          | M01    | 4        | 1 (ABIERTO)  |

---

## Usuarios y Seguridad

### Tabla: `Usuario`
- **Propósito:** Representa al personal (meseros, cajeros, admin).  
- **Relaciones:** FK a `Empresa`. Asociado a `Turno`, `Pedido`, `Pago`.  
- **Ejemplo de datos:**

| IdUsuario | NombreCompleto   | Correo             | Activo |
|-----------|------------------|--------------------|--------|
| 1         | Juan Pérez       | juan@xyz.com       | 1      |

---

### Tabla: `Rol`
- **Propósito:** Define roles de seguridad.  
- **Relaciones:** Asociado a `UsuarioRol`.  
- **Ejemplo de datos:**

| IdRol | Nombre    | IsSystem | IsAssignable |
|-------|-----------|----------|--------------|
| 1     | Cajero    | 0        | 1            |
| 2     | Admin     | 1        | 0            |

---

### Tabla: `UsuarioRol`
- **Propósito:** Relaciona usuarios y roles.  
- **Ejemplo de datos:**

| IdUsuario | IdRol |
|-----------|-------|
| 1         | 1     |

---

### Tabla: `Credencial`
- **Propósito:** Almacena credenciales de acceso.  
- **Relaciones:** FK a `Usuario` y `Catalog`.  
- **Ejemplo de datos:**

| IdUsuario | TipoItemId | Hash               |
|-----------|------------|--------------------|
| 1         | 1 (PIN)    | abcd1234hashed     |

---

### Tabla: `Turno`
- **Propósito:** Controla los turnos de trabajo.  
- **Relaciones:** FK a `Usuario`, `Sucursal`, usado en `MovimientoCaja`.  
- **Ejemplo de datos:**

| IdTurno | IdUsuario | IdSucursal | Apertura             | CajaInicial |
|---------|-----------|------------|----------------------|-------------|
| 1       | 1         | 1          | 2025-09-18 08:00:00  | 1000.00     |

---

## Menú / Precios / Modificadores

### Tabla: `Menu`
- **Propósito:** Define menús por sucursal.  
- **Ejemplo de datos:**

| IdMenu | IdSucursal | Nombre      |
|--------|------------|-------------|
| 1      | 1          | Menú Lunch  |

---

### Tabla: `CategoriaMenu`
- **Propósito:** Agrupa productos dentro del menú.  
- **Ejemplo de datos:**

| IdCategoria | IdMenu | Nombre   | Orden |
|-------------|--------|----------|-------|
| 1           | 1      | Entradas | 1     |

---

### Tabla: `Producto`
- **Propósito:** Representa platillos o bebidas.  
- **Ejemplo de datos:**

| IdProducto | IdMenu | IdCategoria | Codigo | Nombre     |
|------------|--------|-------------|--------|------------|
| 1          | 1      | 1           | TACO01 | Tacos      |

---

### Tabla: `VarianteProducto`
- **Propósito:** Define variantes del producto (tamaño, sabor).  
- **Ejemplo de datos:**

| IdVariante | IdProducto | Nombre   | Codigo |
|------------|------------|----------|--------|
| 1          | 1          | Grande   | TACG   |

---

### Tabla: `Precio`
- **Propósito:** Define precios dinámicos.  
- **Ejemplo de datos:**

| IdPrecio | IdVariante | Monto  | Moneda | ImpuestoItemId |
|----------|------------|--------|--------|----------------|
| 1        | 1          | 80.00  | MXN    | 1 (IVA)        |

---

### Tabla: `GrupoModificador`
- **Propósito:** Agrupa opciones de personalización.  
- **Ejemplo de datos:**

| IdGrupo | IdProducto | Nombre       | MaxSeleccion |
|---------|------------|--------------|--------------|
| 1       | 1          | Salsas       | 2            |

---

### Tabla: `OpcionModificador`
- **Propósito:** Opciones dentro de un grupo.  
- **Ejemplo de datos:**

| IdOpcion | IdGrupo | Nombre      | PrecioExtra |
|----------|---------|-------------|-------------|
| 1        | 1       | Salsa Roja  | 0.00        |

---

## Pedidos

### Tabla: `Pedido`
- **Propósito:** Pedido abierto en mesa/cliente.  
- **Ejemplo de datos:**

| IdPedido | IdSucursal | IdMesa | AbiertoPor | EstadoItemId |
|----------|------------|--------|------------|--------------|
| 1001     | 1          | 1      | 1          | 1 (ABIERTO)  |

---

### Tabla: `PedidoAsiento`
- **Propósito:** Divide pedido por asiento.  
- **Ejemplo de datos:**

| IdAsiento | IdPedido | NumeroAsiento |
|-----------|----------|---------------|
| 1         | 1001     | 1             |

---

### Tabla: `PedidoDetalle`
- **Propósito:** Ítems de pedido.  
- **Ejemplo de datos:**

| IdDetalle | IdPedido | IdProducto | Cantidad | PrecioUnitario |
|-----------|----------|------------|----------|----------------|
| 1         | 1001     | 1          | 2        | 80.00          |

---

### Tabla: `PedidoModificador`
- **Propósito:** Opciones extra elegidas.  
- **Ejemplo de datos:**

| IdModificador | IdDetalle | IdOpcion | PrecioExtra |
|---------------|-----------|----------|-------------|
| 1             | 1         | 1        | 0.00        |

---

### Tabla: `EventoPedido`
- **Propósito:** Historial de eventos del pedido.  
- **Ejemplo de datos:**

| IdEvento | IdPedido | Fecha               | TipoEvento |
|----------|----------|---------------------|------------|
| 1        | 1001     | 2025-09-18 13:30:00 | CambioEstado |

---

## KDS (Cocina)

### Tabla: `EstacionCocina`
- **Propósito:** Estaciones de preparación.  
- **Ejemplo de datos:**

| IdEstacion | IdSucursal | Nombre    |
|------------|------------|-----------|
| 1          | 1          | Cocina Fría |

---

### Tabla: `TicketCocina`
- **Propósito:** Ticket generado para la cocina.  
- **Ejemplo de datos:**

| IdTicket | IdEstacion | IdPedido | EstadoItemId |
|----------|------------|----------|--------------|
| 1        | 1          | 1001     | 1 (EN COLA)  |

---

### Tabla: `TicketDetalle`
- **Propósito:** Detalles de ticket en cocina.  
- **Ejemplo de datos:**

| IdTicketDetalle | IdTicket | IdDetalle | EstadoItemId |
|-----------------|----------|-----------|--------------|
| 1               | 1        | 1         | 1 (PENDIENTE)|

---

## Cobro

### Tabla: `Cuenta`
- **Propósito:** Resumen de cargos del pedido.  
- **Ejemplo de datos:**

| IdCuenta | IdPedido | Subtotal | Total | EstadoItemId |
|----------|----------|----------|-------|--------------|
| 1        | 1001     | 160.00   | 185.60| 1 (ABIERTA)  |

---

### Tabla: `DetalleCuenta`
- **Propósito:** Líneas de detalle de la cuenta.  
- **Ejemplo de datos:**

| IdDetalleCuenta | IdCuenta | TipoOrigen | Monto |
|-----------------|----------|------------|-------|
| 1               | 1        | Item       | 160.00|

---

### Tabla: `Pago`
- **Propósito:** Registro de pagos.  
- **Ejemplo de datos:**

| IdPago | IdCuenta | Monto | MetodoItemId | Propina |
|--------|----------|-------|--------------|---------|
| 1      | 1        | 185.60| 1 (EFECTIVO) | 20.00   |

---

### Tabla: `DescuentoAplicado`
- **Propósito:** Registra descuentos otorgados.  
- **Ejemplo de datos:**

| IdDescuentoAplicado | IdCuenta | TipoItemId | Valor |
|---------------------|----------|------------|-------|
| 1                   | 1        | 1 (Promo)  | 20.00 |

---

## Caja

### Tabla: `MovimientoCaja`
- **Propósito:** Movimientos de efectivo.  
- **Ejemplo de datos:**

| IdMovimiento | IdTurno | Tipo    | Monto |
|--------------|---------|---------|-------|
| 1            | 1       | Ingreso | 100.00|

---

### Tabla: `CorteCaja`
- **Propósito:** Corte de caja diario o por turno.  
- **Ejemplo de datos:**

| IdCorteCaja | IdSucursal | TotalVentas | Diferencia |
|-------------|------------|-------------|------------|
| 1           | 1          | 1000.00     | 0.00       |

---

## Clientes

### Tabla: `Cliente`
- **Propósito:** Información de clientes.  
- **Ejemplo de datos:**

| IdCliente | IdEmpresa | Nombre        | Telefono   |
|-----------|-----------|---------------|------------|
| 1         | 1         | Carlos López  | 555-123456 |

---

## Flujos Operativos

### Crear Pedido
1. `Usuario` abre un `Pedido` en una `Mesa`.  
2. Se agregan `PedidoDetalle` y `PedidoAsiento`.  
3. Se disparan `EventoPedido` y se generan `TicketCocina`.  

### Procesar Pago
1. Se genera una `Cuenta`.  
2. Se calculan subtotales, impuestos y cargos.  
3. Se registran `Pago` y `DescuentoAplicado`.  
4. Estado de la cuenta cambia a “Pagada”.  

### Cierre de Caja
1. `Turno` registra `MovimientoCaja`.  
2. Se genera `CorteCaja`.  
3. Se concilia `Declarado` vs `CajaEsperada`.  

---

# Conclusión

El modelo cubre de manera integral la operación de un restaurante moderno.  
Cada módulo está conectado a través de catálogos, lo que otorga flexibilidad para soportar distintos tipos de pedidos, pagos, impuestos y reglas de negocio.
