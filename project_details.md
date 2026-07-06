# MesaFacil.API - Especificaciones y Pormenores del Proyecto

Este documento provee un resumen técnico sumamente detallado de la arquitectura, base de datos y flujos de negocio de **MesaFacil.API**. Está diseñado para servir como fuente de contexto estructurada para modelos de lenguaje (LLMs), permitiendo entender la estructura del proyecto rápidamente y optimizar el uso de tokens.

---

## 1. Arquitectura del Proyecto

El sistema está estructurado bajo los principios de **Clean Architecture (Onion Architecture)**, aislando las reglas de negocio del framework y los detalles de infraestructura. Se compone de los siguientes proyectos (carpetas):

*   **Domain**:
    *   **Propósito**: Núcleo del sistema. Contiene las entidades puras del negocio y enumeraciones.
    *   **Herencia**: Casi todas las entidades heredan de `BaseEntity` (que expone `Id` e `IsActive`) y `BaseAuditableEntity` (que añade metadatos de auditoría como `CreatedAt`, `CreatedBy`, `UpdatedAt` y `UpdatedBy`).
*   **Interface**:
    *   **Propósito**: Contiene los contratos de abstracción de repositorios y de casos de uso (capa de aplicación).
    *   **Componentes Clave**: Interfaces como `IUsuarioRepository`, `IAuthApplication`, `IUnitOfWork`, etc.
*   **Persistence**:
    *   **Propósito**: Capa de datos y acceso físico.
    *   **Tecnologías**: Entity Framework Core con PostgreSQL (vía `Npgsql`).
    *   **Componentes Clave**:
        *   `ApplicationDbContext`: Mapeador de la base de datos.
        *   `Configurations/`: Clases de Fluent API para configurar restricciones de llaves foráneas y tipos de columnas en tablas.
        *   `Repositories/`: Implementación de repositorios de acceso a datos y el patrón de Unit of Work (`UnitOfWork`).
        *   `DatabaseInitializer`: Inicializador/Seed de la base de datos (creación de roles, administrador inicial, catálogos y formularios).
*   **UseCases**:
    *   **Propósito**: Lógica de aplicación. Coordina el flujo de datos hacia y desde las entidades.
    *   **Componentes Clave**:
        *   Implementaciones de servicios de aplicación (ej. `AuthApplication`, `UsuarioApplication`).
        *   `DTOs/`: Modelos de transferencia de datos de entrada/salida (ej. `UsuarioDTO`, `AuthResponseDTO`).
        *   `Common/Mapping/MappingsProfile`: Mapeo de AutoMapper entre Entidades y DTOs.
*   **Validator**:
    *   **Propósito**: Capa de validación desacoplada.
    *   **Tecnología**: FluentValidation.
    *   **Componentes Clave**: Clases que heredan de `AbstractValidator<T>` (ej. `LoginRequestValidator`, `UsuarioDTOValidator`).
*   **WebApi**:
    *   **Propósito**: Punto de entrada de la aplicación HTTP.
    *   **Componentes Clave**:
        *   **Controladores ASP.NET Core (MVC Controllers)**: Controlan las peticiones REST expuestas a los clientes (ej. `AuthController`, `UsuarioController`).
        *   `Program.cs`: Configuración de servicios (DI), autenticación/autorización JWT, logger y middlewares.
*   **Common / Logging**:
    *   **Propósito**: Proyectos transversales para soporte de logs y utilidades como hashing de contraseñas (`IPasswordHasher`).

---

## 2. Modelo y Esquema de Base de Datos

La base de datos maneja la operación de un sistema POS (Point of Sale) para restaurantes. El esquema de datos está organizado en los siguientes módulos conceptuales:

### Módulo de Catálogos (Catalog / CatalogItem)
*   **Catalog**: Define los catálogos maestros de la aplicación (ej. `EstadoPedido`, `MetodoPago`, `TipoImpuesto`, `TipoCredencial`).
*   **CatalogItem**: Los elementos concretos de cada catálogo (ej. `ABIERTO`, `CERRADO`, `EFECTIVO`, `PASSWORD`, `PIN`).

### Módulo de Núcleo (Core)
*   **Empresa**: Organización dueña de los restaurantes.
*   **Sucursal**: Ubicaciones geográficas de cada restaurante de una empresa.
*   **Area**: Zonas de la sucursal (ej. terraza, barra, salón).
*   **Mesa**: Las mesas físicas asignadas a áreas con un número de asientos y su estado de ocupación actual.

### Módulo de Usuarios y Seguridad
*   **Usuario**: Personal que interactúa con el sistema (meseros, cajeros, administradores).
*   **Rol**: Roles del sistema (`Admin`, `Manager`, `Mesero`).
*   **UsuarioRol**: Relación de muchos a muchos para asignar múltiples roles a usuarios.
*   **Credencial**: Almacena las llaves o hashes de autenticación de los usuarios (referencia a `CatCredencial` para soportar dinámicamente contraseñas, PINs, etc.).
*   **Turno**: Controla la sesión operativa del usuario en caja, necesaria para abrir operaciones.

### Módulo de Menús y Productos
*   **Menu**: Catálogo de productos asignados a una sucursal.
*   **CategoriaMenu**: Agrupación dentro del menú (Entradas, Bebidas, Fuertes).
*   **Producto**: Fichas de comida o bebidas.
*   **VarianteProducto**: Opciones físicas de presentación (Chico, Mediano, Grande).
*   **Precio**: Tarifas numéricas enlazadas a variantes.
*   **GrupoModificador** y **OpcionModificador**: Modificaciones de recetas (ej. "Término de Carne", con opciones "Término Medio", "Bien Cocido").

### Módulo de Pedidos y Cocina
*   **Pedido**: Cuenta activa iniciada en una mesa por un mesero.
*   **PedidoAsiento**: División de comensales en la mesa.
*   **PedidoDetalle**: Platillos cargados al pedido con su cantidad y notas de preparación.
*   **PedidoModificador**: Modificadores seleccionados por detalle.
*   **EstacionCocina**: Destinos físicos en cocina (ej. Cocina Caliente, Barra).
*   **TicketCocina** / **TicketDetalle**: Órdenes que visualizan los cocineros en sus monitores de cocina (KDS).

### Módulo de Cobros y Caja
*   **Cuenta**: El ticket de cobro generado al cerrar un pedido.
*   **DetalleCuenta**: Desglose de consumo.
*   **Pago**: Registro de la transacción monetaria (efectivo, tarjeta, etc.).
*   **DescuentoAplicado**: Reducciones en la cuenta.
*   **MovimientoCaja**: Entradas y salidas operativas de dinero durante el turno.
*   **CorteCaja**: Arqueo y conciliación de caja al cierre de operaciones.

---

## 3. Flujo de Autenticación y Credenciales

El sistema implementa un flujo de credenciales flexible y extensible:

1.  **Autenticación JWT**:
    *   La API genera un token JWT firmado al iniciar sesión exitosamente, inyectando claims de usuario (sub, email, uid, empresa_id, nombre, sucursal_id, turno_abierto, permisos asignados y versión de seguridad).
2.  **Soporte Multicredencial**:
    *   Las credenciales no se limitan a contraseñas tradicionales (`PASSWORD`). Se soportan métodos operativos como códigos numéricos (`PIN`) o integraciones futuras.
    *   La validación se realiza comparando hashes criptográficos generados mediante PBKDF2 (SHA256) con sal única.
3.  **Endoints de Acceso**:
    *   `POST api/auth/login`: Autenticación tradicional mediante usuario/correo y contraseña.
    *   `POST api/auth/login-pin`: Autenticación rápida operativa mediante correo y PIN.

---

## 4. Estructura de Automatización

El proyecto cuenta con generadores automáticos de código para agilizar el desarrollo de nuevas entidades:
*   **Linux / macOS**: `./scripts/generate_entity.sh EntityName --plural EntityPlural`
*   **Windows**: `pwsh ./scripts/generate_entity.ps1 -Name EntityName -Plural EntityPlural`

Estos scripts crean automáticamente la entidad en `Domain`, configuran la base de datos en `Persistence`, generan DTOs, validadores, mapeos de AutoMapper, servicios de aplicación en `UseCases` y registran las inyecciones de dependencia en el contenedor de servicios de la API.
