using Domain.Entities;
using DTO.AccesoRuta;
using DTO.Area;
using DTO.CatCredencial;
using DTO.CategoriaMenu;
using DTO.Cliente;
using DTO.CorteCaja;
using DTO.Credencial;
using DTO.Cuenta;
using DTO.DescuentoAplicado;
using DTO.DetalleCuenta;
using DTO.Empresa;
using DTO.EstacionCocina;
using DTO.EventoPedido;
using DTO.FormField;
using DTO.Formulario;
using DTO.GenericCatalog;
using DTO.GrupoModificador;
using DTO.Menu;
using DTO.Mesa;
using DTO.MovimientoCaja;
using DTO.OpcionModificador;
using DTO.Pago;
using DTO.Pedido;
using DTO.PedidoAsiento;
using DTO.PedidoDetalle;
using DTO.PedidoModificador;
using DTO.Precio;
using DTO.Producto;
using DTO.Rol;
using DTO.RolAccesoRuta;
using DTO.Sucursal;
using DTO.TicketCocina;
using DTO.TicketDetalle;
using DTO.TiposPedido;
using DTO.Turno;
using DTO.Usuario;
using DTO.UsuarioRol;
using DTO.VarianteProducto;

namespace Interface.Mapping;

public interface IAppMapper
{
    // AccesoRuta
    AccesoRuta ToEntity(AccesoRutaDTO dto);
    AccesoRutaDTO ToDTO(AccesoRuta entity);
    List<AccesoRutaDTO> ToDTOList(IEnumerable<AccesoRuta> entities);

    // Area
    Area ToEntity(AreaDTO dto);
    AreaDTO ToDTO(Area entity);
    List<AreaDTO> ToDTOList(IEnumerable<Area> entities);

    // CatCredencial
    CatCredencial ToEntity(CatCredencialDTO dto);
    CatCredencialDTO ToDTO(CatCredencial entity);
    List<CatCredencialDTO> ToDTOList(IEnumerable<CatCredencial> entities);

    // CategoriaMenu
    CategoriaMenu ToEntity(CategoriaMenuDTO dto);
    CategoriaMenuDTO ToDTO(CategoriaMenu entity);
    List<CategoriaMenuDTO> ToDTOList(IEnumerable<CategoriaMenu> entities);

    // CatTipoPedido
    CatTipoPedido ToEntity(TipoPedidoDTO dto);
    TipoPedidoDTO ToDTO(CatTipoPedido entity);
    List<TipoPedidoDTO> ToDTOList(IEnumerable<CatTipoPedido> entities);

    // Cliente
    Cliente ToEntity(ClienteDTO dto);
    ClienteDTO ToDTO(Cliente entity);
    List<ClienteDTO> ToDTOList(IEnumerable<Cliente> entities);

    // CorteCaja
    CorteCaja ToEntity(CorteCajaDTO dto);
    CorteCajaDTO ToDTO(CorteCaja entity);
    List<CorteCajaDTO> ToDTOList(IEnumerable<CorteCaja> entities);

    // Credencial
    Credencial ToEntity(CredencialDTO dto);
    CredencialDTO ToDTO(Credencial entity);
    List<CredencialDTO> ToDTOList(IEnumerable<Credencial> entities);

    // Cuenta
    Cuenta ToEntity(CuentaDTO dto);
    CuentaDTO ToDTO(Cuenta entity);
    List<CuentaDTO> ToDTOList(IEnumerable<Cuenta> entities);

    // DescuentoAplicado
    DescuentoAplicado ToEntity(DescuentoAplicadoDTO dto);
    DescuentoAplicadoDTO ToDTO(DescuentoAplicado entity);
    List<DescuentoAplicadoDTO> ToDTOList(IEnumerable<DescuentoAplicado> entities);

    // DetalleCuenta
    DetalleCuenta ToEntity(DetalleCuentaDTO dto);
    DetalleCuentaDTO ToDTO(DetalleCuenta entity);
    List<DetalleCuentaDTO> ToDTOList(IEnumerable<DetalleCuenta> entities);

    // Empresa
    Empresa ToEntity(EmpresaDTO dto);
    EmpresaDTO ToDTO(Empresa entity);
    List<EmpresaDTO> ToDTOList(IEnumerable<Empresa> entities);

    // EstacionCocina
    EstacionCocina ToEntity(EstacionCocinaDTO dto);
    EstacionCocinaDTO ToDTO(EstacionCocina entity);
    List<EstacionCocinaDTO> ToDTOList(IEnumerable<EstacionCocina> entities);

    // EventoPedido
    EventoPedido ToEntity(EventoPedidoDTO dto);
    EventoPedidoDTO ToDTO(EventoPedido entity);
    List<EventoPedidoDTO> ToDTOList(IEnumerable<EventoPedido> entities);

    // FormField
    FormField ToEntity(FormFieldDTO dto);
    FormFieldDTO ToDTO(FormField entity);
    List<FormFieldDTO> ToDTOList(IEnumerable<FormField> entities);

    // Formulario
    Formulario ToEntity(FormularioDTO dto);
    FormularioDTO ToDTO(Formulario entity);
    List<FormularioDTO> ToDTOList(IEnumerable<Formulario> entities);

    // GrupoModificador
    GrupoModificador ToEntity(GrupoModificadorDTO dto);
    GrupoModificadorDTO ToDTO(GrupoModificador entity);
    List<GrupoModificadorDTO> ToDTOList(IEnumerable<GrupoModificador> entities);

    // Menu
    Menu ToEntity(MenuDTO dto);
    MenuDTO ToDTO(Menu entity);
    List<MenuDTO> ToDTOList(IEnumerable<Menu> entities);

    // Mesa
    Mesa ToEntity(MesaDTO dto);
    MesaDTO ToDTO(Mesa entity);
    List<MesaDTO> ToDTOList(IEnumerable<Mesa> entities);

    // MovimientoCaja
    MovimientoCaja ToEntity(MovimientoCajaDTO dto);
    MovimientoCajaDTO ToDTO(MovimientoCaja entity);
    List<MovimientoCajaDTO> ToDTOList(IEnumerable<MovimientoCaja> entities);

    // OpcionModificador
    OpcionModificador ToEntity(OpcionModificadorDTO dto);
    OpcionModificadorDTO ToDTO(OpcionModificador entity);
    List<OpcionModificadorDTO> ToDTOList(IEnumerable<OpcionModificador> entities);

    // Pago
    Pago ToEntity(PagoDTO dto);
    PagoDTO ToDTO(Pago entity);
    List<PagoDTO> ToDTOList(IEnumerable<Pago> entities);

    // Pedido
    Pedido ToEntity(PedidoDTO dto);
    PedidoDTO ToDTO(Pedido entity);
    List<PedidoDTO> ToDTOList(IEnumerable<Pedido> entities);
    Pedido ToEntity(CrearPedidoRequestDTO dto);
    CrearPedidoRequestDTO ToCrearPedidoRequestDTO(Pedido entity);

    // PedidoAsiento
    PedidoAsiento ToEntity(PedidoAsientoDTO dto);
    PedidoAsientoDTO ToDTO(PedidoAsiento entity);
    List<PedidoAsientoDTO> ToDTOList(IEnumerable<PedidoAsiento> entities);

    // PedidoDetalle
    PedidoDetalle ToEntity(PedidoDetalleDTO dto);
    PedidoDetalleDTO ToDTO(PedidoDetalle entity);
    List<PedidoDetalleDTO> ToDTOList(IEnumerable<PedidoDetalle> entities);
    PedidoDetalle ToEntity(CrearPedidoDetalleDTO dto);
    CrearPedidoDetalleDTO ToCrearPedidoDetalleDTO(PedidoDetalle entity);

    // PedidoModificador
    PedidoModificador ToEntity(PedidoModificadorDTO dto);
    PedidoModificadorDTO ToDTO(PedidoModificador entity);
    List<PedidoModificadorDTO> ToDTOList(IEnumerable<PedidoModificador> entities);

    // Precio
    Precio ToEntity(PrecioDTO dto);
    PrecioDTO ToDTO(Precio entity);
    List<PrecioDTO> ToDTOList(IEnumerable<Precio> entities);

    // Producto
    Producto ToEntity(ProductoDTO dto);
    ProductoDTO ToDTO(Producto entity);
    List<ProductoDTO> ToDTOList(IEnumerable<Producto> entities);

    // Rol
    Rol ToEntity(RolDTO dto);
    RolDTO ToDTO(Rol entity);
    List<RolDTO> ToDTOList(IEnumerable<Rol> entities);

    // RolAccesoRuta
    RolAccesoRuta ToEntity(RolAccesoRutaDTO dto);
    RolAccesoRutaDTO ToDTO(RolAccesoRuta entity);
    List<RolAccesoRutaDTO> ToDTOList(IEnumerable<RolAccesoRuta> entities);

    // Sucursal
    Sucursal ToEntity(SucursalDTO dto);
    SucursalDTO ToDTO(Sucursal entity);
    List<SucursalDTO> ToDTOList(IEnumerable<Sucursal> entities);

    // TicketCocina
    TicketCocina ToEntity(TicketCocinaDTO dto);
    TicketCocinaDTO ToDTO(TicketCocina entity);
    List<TicketCocinaDTO> ToDTOList(IEnumerable<TicketCocina> entities);

    // TicketDetalle
    TicketDetalle ToEntity(TicketDetalleDTO dto);
    TicketDetalleDTO ToDTO(TicketDetalle entity);
    List<TicketDetalleDTO> ToDTOList(IEnumerable<TicketDetalle> entities);

    // Turno
    Turno ToEntity(TurnoDTO dto);
    TurnoDTO ToDTO(Turno entity);
    List<TurnoDTO> ToDTOList(IEnumerable<Turno> entities);

    // Usuario
    Usuario ToEntity(UsuarioDTO dto);
    UsuarioDTO ToDTO(Usuario entity);
    List<UsuarioDTO> ToDTOList(IEnumerable<Usuario> entities);

    // UsuarioRol
    UsuarioRol ToEntity(UsuarioRolDTO dto);
    UsuarioRolDTO ToDTO(UsuarioRol entity);
    List<UsuarioRolDTO> ToDTOList(IEnumerable<UsuarioRol> entities);

    // VarianteProducto
    VarianteProducto ToEntity(VarianteProductoDTO dto);
    VarianteProductoDTO ToDTO(VarianteProducto entity);
    List<VarianteProductoDTO> ToDTOList(IEnumerable<VarianteProducto> entities);

    // Catálogos genéricos
    GenericCatalogDTO ToGenericCatalogDTO(ICatalogEntity entity);
    List<GenericCatalogDTO> ToGenericCatalogDTOList(IEnumerable<ICatalogEntity> entities);
    TEntity ToGenericCatalogEntity<TEntity>(GenericCatalogDTO dto) where TEntity : BaseAuditableEntity, ICatalogEntity, new();

    // Métodos tipados de conveniencia Map<T> para compatibilidad limpia y sin reflexión
    TDestination Map<TDestination>(object? source);
}
