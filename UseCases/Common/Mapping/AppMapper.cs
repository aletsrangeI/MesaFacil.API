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
using Interface.Mapping;
using Riok.Mapperly.Abstractions;

namespace UseCases.Common.Mapping;

[Mapper]
public partial class AppMapper : IAppMapper
{
    // AccesoRuta
    public partial AccesoRuta ToEntity(AccesoRutaDTO dto);
    public partial AccesoRutaDTO ToDTO(AccesoRuta entity);
    public partial List<AccesoRutaDTO> ToDTOList(IEnumerable<AccesoRuta> entities);

    // Area
    public partial Area ToEntity(AreaDTO dto);
    public partial AreaDTO ToDTO(Area entity);
    public partial List<AreaDTO> ToDTOList(IEnumerable<Area> entities);

    // CatCredencial
    public partial CatCredencial ToEntity(CatCredencialDTO dto);
    public partial CatCredencialDTO ToDTO(CatCredencial entity);
    public partial List<CatCredencialDTO> ToDTOList(IEnumerable<CatCredencial> entities);

    // CategoriaMenu
    public partial CategoriaMenu ToEntity(CategoriaMenuDTO dto);
    public partial CategoriaMenuDTO ToDTO(CategoriaMenu entity);
    public partial List<CategoriaMenuDTO> ToDTOList(IEnumerable<CategoriaMenu> entities);

    // CatTipoPedido
    public partial CatTipoPedido ToEntity(TipoPedidoDTO dto);
    public partial TipoPedidoDTO ToDTO(CatTipoPedido entity);
    public partial List<TipoPedidoDTO> ToDTOList(IEnumerable<CatTipoPedido> entities);

    // Cliente
    public partial Cliente ToEntity(ClienteDTO dto);
    public partial ClienteDTO ToDTO(Cliente entity);
    public partial List<ClienteDTO> ToDTOList(IEnumerable<Cliente> entities);

    // CorteCaja
    public partial CorteCaja ToEntity(CorteCajaDTO dto);
    public partial CorteCajaDTO ToDTO(CorteCaja entity);
    public partial List<CorteCajaDTO> ToDTOList(IEnumerable<CorteCaja> entities);

    // Credencial
    public partial Credencial ToEntity(CredencialDTO dto);
    public partial CredencialDTO ToDTO(Credencial entity);
    public partial List<CredencialDTO> ToDTOList(IEnumerable<Credencial> entities);

    // Cuenta
    public partial Cuenta ToEntity(CuentaDTO dto);
    public partial CuentaDTO ToDTO(Cuenta entity);
    public partial List<CuentaDTO> ToDTOList(IEnumerable<Cuenta> entities);

    // DescuentoAplicado
    public partial DescuentoAplicado ToEntity(DescuentoAplicadoDTO dto);
    public partial DescuentoAplicadoDTO ToDTO(DescuentoAplicado entity);
    public partial List<DescuentoAplicadoDTO> ToDTOList(IEnumerable<DescuentoAplicado> entities);

    // DetalleCuenta
    public partial DetalleCuenta ToEntity(DetalleCuentaDTO dto);
    public partial DetalleCuentaDTO ToDTO(DetalleCuenta entity);
    public partial List<DetalleCuentaDTO> ToDTOList(IEnumerable<DetalleCuenta> entities);

    // Empresa
    public partial Empresa ToEntity(EmpresaDTO dto);
    public partial EmpresaDTO ToDTO(Empresa entity);
    public partial List<EmpresaDTO> ToDTOList(IEnumerable<Empresa> entities);

    // EstacionCocina
    public partial EstacionCocina ToEntity(EstacionCocinaDTO dto);
    public partial EstacionCocinaDTO ToDTO(EstacionCocina entity);
    public partial List<EstacionCocinaDTO> ToDTOList(IEnumerable<EstacionCocina> entities);

    // EventoPedido
    public partial EventoPedido ToEntity(EventoPedidoDTO dto);
    public partial EventoPedidoDTO ToDTO(EventoPedido entity);
    public partial List<EventoPedidoDTO> ToDTOList(IEnumerable<EventoPedido> entities);

    // FormField
    [MapperIgnoreTarget(nameof(FormField.Formulario))]
    public partial FormField ToEntity(FormFieldDTO dto);
    public partial FormFieldDTO ToDTO(FormField entity);
    public partial List<FormFieldDTO> ToDTOList(IEnumerable<FormField> entities);

    // Formulario
    public partial Formulario ToEntity(FormularioDTO dto);
    public partial FormularioDTO ToDTO(Formulario entity);
    public partial List<FormularioDTO> ToDTOList(IEnumerable<Formulario> entities);

    // GrupoModificador
    [MapProperty(nameof(GrupoModificadorDTO.Activo), nameof(GrupoModificador.IsActive))]
    public partial GrupoModificador ToEntity(GrupoModificadorDTO dto);
    [MapProperty(nameof(GrupoModificador.IsActive), nameof(GrupoModificadorDTO.Activo))]
    public partial GrupoModificadorDTO ToDTO(GrupoModificador entity);
    public partial List<GrupoModificadorDTO> ToDTOList(IEnumerable<GrupoModificador> entities);

    // Menu
    public partial Menu ToEntity(MenuDTO dto);
    public partial MenuDTO ToDTO(Menu entity);
    public partial List<MenuDTO> ToDTOList(IEnumerable<Menu> entities);

    // Mesa
    public partial Mesa ToEntity(MesaDTO dto);
    public partial MesaDTO ToDTO(Mesa entity);
    public partial List<MesaDTO> ToDTOList(IEnumerable<Mesa> entities);

    // MovimientoCaja
    public partial MovimientoCaja ToEntity(MovimientoCajaDTO dto);
    public partial MovimientoCajaDTO ToDTO(MovimientoCaja entity);
    public partial List<MovimientoCajaDTO> ToDTOList(IEnumerable<MovimientoCaja> entities);

    // OpcionModificador
    [MapProperty(nameof(OpcionModificadorDTO.Activo), nameof(OpcionModificador.IsActive))]
    public partial OpcionModificador ToEntity(OpcionModificadorDTO dto);
    [MapProperty(nameof(OpcionModificador.IsActive), nameof(OpcionModificadorDTO.Activo))]
    public partial OpcionModificadorDTO ToDTO(OpcionModificador entity);
    public partial List<OpcionModificadorDTO> ToDTOList(IEnumerable<OpcionModificador> entities);

    // Pago
    public partial Pago ToEntity(PagoDTO dto);
    public partial PagoDTO ToDTO(Pago entity);
    public partial List<PagoDTO> ToDTOList(IEnumerable<Pago> entities);

    // Pedido
    public partial Pedido ToEntity(PedidoDTO dto);
    public partial PedidoDTO ToDTO(Pedido entity);
    public partial List<PedidoDTO> ToDTOList(IEnumerable<Pedido> entities);
    public partial Pedido ToEntity(CrearPedidoRequestDTO dto);
    public partial CrearPedidoRequestDTO ToCrearPedidoRequestDTO(Pedido entity);

    // PedidoAsiento
    public partial PedidoAsiento ToEntity(PedidoAsientoDTO dto);
    public partial PedidoAsientoDTO ToDTO(PedidoAsiento entity);
    public partial List<PedidoAsientoDTO> ToDTOList(IEnumerable<PedidoAsiento> entities);

    // PedidoDetalle
    public partial PedidoDetalle ToEntity(PedidoDetalleDTO dto);
    public partial PedidoDetalleDTO ToDTO(PedidoDetalle entity);
    public partial List<PedidoDetalleDTO> ToDTOList(IEnumerable<PedidoDetalle> entities);

    public PedidoDetalle ToEntity(CrearPedidoDetalleDTO dto)
    {
        if (dto == null) return null!;
        var entity = MapCrearPedidoDetalleToPedidoDetalleInternal(dto);
        if (dto.OpcionesModificador != null && dto.OpcionesModificador.Any())
        {
            entity.Modificadores = dto.OpcionesModificador.Select(id => new PedidoModificador
            {
                IdOpcion = id,
                IsActive = true,
                CreatedBy = "system"
            }).ToList();
        }
        else
        {
            entity.Modificadores = new List<PedidoModificador>();
        }
        return entity;
    }

    [MapperIgnoreTarget(nameof(PedidoDetalle.Modificadores))]
    private partial PedidoDetalle MapCrearPedidoDetalleToPedidoDetalleInternal(CrearPedidoDetalleDTO dto);

    public partial CrearPedidoDetalleDTO ToCrearPedidoDetalleDTO(PedidoDetalle entity);

    // PedidoModificador
    public partial PedidoModificador ToEntity(PedidoModificadorDTO dto);
    public partial PedidoModificadorDTO ToDTO(PedidoModificador entity);
    public partial List<PedidoModificadorDTO> ToDTOList(IEnumerable<PedidoModificador> entities);

    // Precio
    [MapProperty(nameof(PrecioDTO.Activo), nameof(Precio.IsActive))]
    public partial Precio ToEntity(PrecioDTO dto);
    [MapProperty(nameof(Precio.IsActive), nameof(PrecioDTO.Activo))]
    public partial PrecioDTO ToDTO(Precio entity);
    public partial List<PrecioDTO> ToDTOList(IEnumerable<Precio> entities);

    // Producto
    public partial Producto ToEntity(ProductoDTO dto);
    public partial ProductoDTO ToDTO(Producto entity);
    public partial List<ProductoDTO> ToDTOList(IEnumerable<Producto> entities);

    // Rol
    public partial Rol ToEntity(RolDTO dto);
    public partial RolDTO ToDTO(Rol entity);
    public partial List<RolDTO> ToDTOList(IEnumerable<Rol> entities);

    // RolAccesoRuta
    public partial RolAccesoRuta ToEntity(RolAccesoRutaDTO dto);
    public partial RolAccesoRutaDTO ToDTO(RolAccesoRuta entity);
    public partial List<RolAccesoRutaDTO> ToDTOList(IEnumerable<RolAccesoRuta> entities);

    // Sucursal
    public partial Sucursal ToEntity(SucursalDTO dto);
    public partial SucursalDTO ToDTO(Sucursal entity);
    public partial List<SucursalDTO> ToDTOList(IEnumerable<Sucursal> entities);

    // TicketCocina
    [MapProperty(nameof(TicketCocinaDTO.Activo), nameof(TicketCocina.IsActive))]
    public partial TicketCocina ToEntity(TicketCocinaDTO dto);
    [MapProperty(nameof(TicketCocina.IsActive), nameof(TicketCocinaDTO.Activo))]
    public partial TicketCocinaDTO ToDTO(TicketCocina entity);
    public partial List<TicketCocinaDTO> ToDTOList(IEnumerable<TicketCocina> entities);

    // TicketDetalle
    [MapProperty(nameof(TicketDetalleDTO.Activo), nameof(TicketDetalle.IsActive))]
    public partial TicketDetalle ToEntity(TicketDetalleDTO dto);
    [MapProperty(nameof(TicketDetalle.IsActive), nameof(TicketDetalleDTO.Activo))]
    public partial TicketDetalleDTO ToDTO(TicketDetalle entity);
    public partial List<TicketDetalleDTO> ToDTOList(IEnumerable<TicketDetalle> entities);

    // Turno
    public partial Turno ToEntity(TurnoDTO dto);
    public partial TurnoDTO ToDTO(Turno entity);
    public partial List<TurnoDTO> ToDTOList(IEnumerable<Turno> entities);

    // Usuario
    public partial Usuario ToEntity(UsuarioDTO dto);
    public partial UsuarioDTO ToDTO(Usuario entity);
    public partial List<UsuarioDTO> ToDTOList(IEnumerable<Usuario> entities);

    // UsuarioRol
    public partial UsuarioRol ToEntity(UsuarioRolDTO dto);
    public partial UsuarioRolDTO ToDTO(UsuarioRol entity);
    public partial List<UsuarioRolDTO> ToDTOList(IEnumerable<UsuarioRol> entities);

    // VarianteProducto
    [MapProperty(nameof(VarianteProductoDTO.Activo), nameof(VarianteProducto.IsActive))]
    public partial VarianteProducto ToEntity(VarianteProductoDTO dto);
    [MapProperty(nameof(VarianteProducto.IsActive), nameof(VarianteProductoDTO.Activo))]
    public partial VarianteProductoDTO ToDTO(VarianteProducto entity);
    public partial List<VarianteProductoDTO> ToDTOList(IEnumerable<VarianteProducto> entities);

    // GenericCatalog
    public GenericCatalogDTO ToGenericCatalogDTO(ICatalogEntity entity)
    {
        if (entity == null) return null!;
        var dto = new GenericCatalogDTO
        {
            Descripcion = entity.Descripcion
        };

        if (entity is BaseEntity be)
        {
            dto.Id = be.Id;
            dto.IsActive = be.IsActive;
        }

        // Si la entidad concreta expone propiedad Codigo, extraerla
        var propCodigo = entity.GetType().GetProperty("Codigo");
        if (propCodigo != null)
        {
            dto.Codigo = propCodigo.GetValue(entity)?.ToString();
        }

        return dto;
    }

    public List<GenericCatalogDTO> ToGenericCatalogDTOList(IEnumerable<ICatalogEntity> entities)
    {
        if (entities == null) return new List<GenericCatalogDTO>();
        return entities.Select(ToGenericCatalogDTO).ToList();
    }

    public TEntity ToGenericCatalogEntity<TEntity>(GenericCatalogDTO dto) where TEntity : BaseAuditableEntity, ICatalogEntity, new()
    {
        if (dto == null) return null!;
        var entity = new TEntity
        {
            Id = dto.Id,
            Descripcion = dto.Descripcion,
            IsActive = dto.IsActive
        };

        var propCodigo = typeof(TEntity).GetProperty("Codigo");
        if (propCodigo != null && propCodigo.CanWrite)
        {
            propCodigo.SetValue(entity, dto.Codigo);
        }

        return entity;
    }

    public TDestination Map<TDestination>(object? source)
    {
        if (source == null) return default!;

        // AccesoRuta
        if (typeof(TDestination) == typeof(AccesoRuta) && source is AccesoRutaDTO arDto) return (TDestination)(object)ToEntity(arDto);
        if (typeof(TDestination) == typeof(AccesoRutaDTO) && source is AccesoRuta ar) return (TDestination)(object)ToDTO(ar);
        if (typeof(TDestination) == typeof(IEnumerable<AccesoRutaDTO>) && source is IEnumerable<AccesoRuta> arList) return (TDestination)(object)ToDTOList(arList);
        if (typeof(TDestination) == typeof(List<AccesoRutaDTO>) && source is IEnumerable<AccesoRuta> arList2) return (TDestination)(object)ToDTOList(arList2);

        // Area
        if (typeof(TDestination) == typeof(Area) && source is AreaDTO aDto) return (TDestination)(object)ToEntity(aDto);
        if (typeof(TDestination) == typeof(AreaDTO) && source is Area a) return (TDestination)(object)ToDTO(a);
        if (typeof(TDestination) == typeof(IEnumerable<AreaDTO>) && source is IEnumerable<Area> aList) return (TDestination)(object)ToDTOList(aList);
        if (typeof(TDestination) == typeof(List<AreaDTO>) && source is IEnumerable<Area> aList2) return (TDestination)(object)ToDTOList(aList2);

        // CatCredencial
        if (typeof(TDestination) == typeof(CatCredencial) && source is CatCredencialDTO ccDto) return (TDestination)(object)ToEntity(ccDto);
        if (typeof(TDestination) == typeof(CatCredencialDTO) && source is CatCredencial cc) return (TDestination)(object)ToDTO(cc);
        if (typeof(TDestination) == typeof(IEnumerable<CatCredencialDTO>) && source is IEnumerable<CatCredencial> ccList) return (TDestination)(object)ToDTOList(ccList);
        if (typeof(TDestination) == typeof(List<CatCredencialDTO>) && source is IEnumerable<CatCredencial> ccList2) return (TDestination)(object)ToDTOList(ccList2);

        // CategoriaMenu
        if (typeof(TDestination) == typeof(CategoriaMenu) && source is CategoriaMenuDTO cmDto) return (TDestination)(object)ToEntity(cmDto);
        if (typeof(TDestination) == typeof(CategoriaMenuDTO) && source is CategoriaMenu cm) return (TDestination)(object)ToDTO(cm);
        if (typeof(TDestination) == typeof(IEnumerable<CategoriaMenuDTO>) && source is IEnumerable<CategoriaMenu> cmList) return (TDestination)(object)ToDTOList(cmList);
        if (typeof(TDestination) == typeof(List<CategoriaMenuDTO>) && source is IEnumerable<CategoriaMenu> cmList2) return (TDestination)(object)ToDTOList(cmList2);

        // CatTipoPedido
        if (typeof(TDestination) == typeof(CatTipoPedido) && source is TipoPedidoDTO ctpDto) return (TDestination)(object)ToEntity(ctpDto);
        if (typeof(TDestination) == typeof(TipoPedidoDTO) && source is CatTipoPedido ctp) return (TDestination)(object)ToDTO(ctp);
        if (typeof(TDestination) == typeof(IEnumerable<TipoPedidoDTO>) && source is IEnumerable<CatTipoPedido> ctpList) return (TDestination)(object)ToDTOList(ctpList);
        if (typeof(TDestination) == typeof(List<TipoPedidoDTO>) && source is IEnumerable<CatTipoPedido> ctpList2) return (TDestination)(object)ToDTOList(ctpList2);

        // Cliente
        if (typeof(TDestination) == typeof(Cliente) && source is ClienteDTO cDto) return (TDestination)(object)ToEntity(cDto);
        if (typeof(TDestination) == typeof(ClienteDTO) && source is Cliente c) return (TDestination)(object)ToDTO(c);
        if (typeof(TDestination) == typeof(IEnumerable<ClienteDTO>) && source is IEnumerable<Cliente> cList) return (TDestination)(object)ToDTOList(cList);
        if (typeof(TDestination) == typeof(List<ClienteDTO>) && source is IEnumerable<Cliente> cList2) return (TDestination)(object)ToDTOList(cList2);

        // CorteCaja
        if (typeof(TDestination) == typeof(CorteCaja) && source is CorteCajaDTO corteDto) return (TDestination)(object)ToEntity(corteDto);
        if (typeof(TDestination) == typeof(CorteCajaDTO) && source is CorteCaja corte) return (TDestination)(object)ToDTO(corte);
        if (typeof(TDestination) == typeof(IEnumerable<CorteCajaDTO>) && source is IEnumerable<CorteCaja> corteList) return (TDestination)(object)ToDTOList(corteList);
        if (typeof(TDestination) == typeof(List<CorteCajaDTO>) && source is IEnumerable<CorteCaja> corteList2) return (TDestination)(object)ToDTOList(corteList2);

        // Credencial
        if (typeof(TDestination) == typeof(Credencial) && source is CredencialDTO credDto) return (TDestination)(object)ToEntity(credDto);
        if (typeof(TDestination) == typeof(CredencialDTO) && source is Credencial cred) return (TDestination)(object)ToDTO(cred);
        if (typeof(TDestination) == typeof(IEnumerable<CredencialDTO>) && source is IEnumerable<Credencial> credList) return (TDestination)(object)ToDTOList(credList);
        if (typeof(TDestination) == typeof(List<CredencialDTO>) && source is IEnumerable<Credencial> credList2) return (TDestination)(object)ToDTOList(credList2);

        // Cuenta
        if (typeof(TDestination) == typeof(Cuenta) && source is CuentaDTO ctaDto) return (TDestination)(object)ToEntity(ctaDto);
        if (typeof(TDestination) == typeof(CuentaDTO) && source is Cuenta cta) return (TDestination)(object)ToDTO(cta);
        if (typeof(TDestination) == typeof(IEnumerable<CuentaDTO>) && source is IEnumerable<Cuenta> ctaList) return (TDestination)(object)ToDTOList(ctaList);
        if (typeof(TDestination) == typeof(List<CuentaDTO>) && source is IEnumerable<Cuenta> ctaList2) return (TDestination)(object)ToDTOList(ctaList2);

        // DescuentoAplicado
        if (typeof(TDestination) == typeof(DescuentoAplicado) && source is DescuentoAplicadoDTO daDto) return (TDestination)(object)ToEntity(daDto);
        if (typeof(TDestination) == typeof(DescuentoAplicadoDTO) && source is DescuentoAplicado da) return (TDestination)(object)ToDTO(da);
        if (typeof(TDestination) == typeof(IEnumerable<DescuentoAplicadoDTO>) && source is IEnumerable<DescuentoAplicado> daList) return (TDestination)(object)ToDTOList(daList);
        if (typeof(TDestination) == typeof(List<DescuentoAplicadoDTO>) && source is IEnumerable<DescuentoAplicado> daList2) return (TDestination)(object)ToDTOList(daList2);

        // DetalleCuenta
        if (typeof(TDestination) == typeof(DetalleCuenta) && source is DetalleCuentaDTO dcDto) return (TDestination)(object)ToEntity(dcDto);
        if (typeof(TDestination) == typeof(DetalleCuentaDTO) && source is DetalleCuenta dc) return (TDestination)(object)ToDTO(dc);
        if (typeof(TDestination) == typeof(IEnumerable<DetalleCuentaDTO>) && source is IEnumerable<DetalleCuenta> dcList) return (TDestination)(object)ToDTOList(dcList);
        if (typeof(TDestination) == typeof(List<DetalleCuentaDTO>) && source is IEnumerable<DetalleCuenta> dcList2) return (TDestination)(object)ToDTOList(dcList2);

        // Empresa
        if (typeof(TDestination) == typeof(Empresa) && source is EmpresaDTO empDto) return (TDestination)(object)ToEntity(empDto);
        if (typeof(TDestination) == typeof(EmpresaDTO) && source is Empresa emp) return (TDestination)(object)ToDTO(emp);
        if (typeof(TDestination) == typeof(IEnumerable<EmpresaDTO>) && source is IEnumerable<Empresa> empList) return (TDestination)(object)ToDTOList(empList);
        if (typeof(TDestination) == typeof(List<EmpresaDTO>) && source is IEnumerable<Empresa> empList2) return (TDestination)(object)ToDTOList(empList2);

        // EstacionCocina
        if (typeof(TDestination) == typeof(EstacionCocina) && source is EstacionCocinaDTO ecDto) return (TDestination)(object)ToEntity(ecDto);
        if (typeof(TDestination) == typeof(EstacionCocinaDTO) && source is EstacionCocina ec) return (TDestination)(object)ToDTO(ec);
        if (typeof(TDestination) == typeof(IEnumerable<EstacionCocinaDTO>) && source is IEnumerable<EstacionCocina> ecList) return (TDestination)(object)ToDTOList(ecList);
        if (typeof(TDestination) == typeof(List<EstacionCocinaDTO>) && source is IEnumerable<EstacionCocina> ecList2) return (TDestination)(object)ToDTOList(ecList2);

        // EventoPedido
        if (typeof(TDestination) == typeof(EventoPedido) && source is EventoPedidoDTO epDto) return (TDestination)(object)ToEntity(epDto);
        if (typeof(TDestination) == typeof(EventoPedidoDTO) && source is EventoPedido ep) return (TDestination)(object)ToDTO(ep);
        if (typeof(TDestination) == typeof(IEnumerable<EventoPedidoDTO>) && source is IEnumerable<EventoPedido> epList) return (TDestination)(object)ToDTOList(epList);
        if (typeof(TDestination) == typeof(List<EventoPedidoDTO>) && source is IEnumerable<EventoPedido> epList2) return (TDestination)(object)ToDTOList(epList2);

        // FormField
        if (typeof(TDestination) == typeof(FormField) && source is FormFieldDTO ffDto) return (TDestination)(object)ToEntity(ffDto);
        if (typeof(TDestination) == typeof(FormFieldDTO) && source is FormField ff) return (TDestination)(object)ToDTO(ff);
        if (typeof(TDestination) == typeof(IEnumerable<FormFieldDTO>) && source is IEnumerable<FormField> ffList) return (TDestination)(object)ToDTOList(ffList);
        if (typeof(TDestination) == typeof(List<FormFieldDTO>) && source is IEnumerable<FormField> ffList2) return (TDestination)(object)ToDTOList(ffList2);

        // Formulario
        if (typeof(TDestination) == typeof(Formulario) && source is FormularioDTO fDto) return (TDestination)(object)ToEntity(fDto);
        if (typeof(TDestination) == typeof(FormularioDTO) && source is Formulario f) return (TDestination)(object)ToDTO(f);
        if (typeof(TDestination) == typeof(IEnumerable<FormularioDTO>) && source is IEnumerable<Formulario> fList) return (TDestination)(object)ToDTOList(fList);
        if (typeof(TDestination) == typeof(List<FormularioDTO>) && source is IEnumerable<Formulario> fList2) return (TDestination)(object)ToDTOList(fList2);

        // GrupoModificador
        if (typeof(TDestination) == typeof(GrupoModificador) && source is GrupoModificadorDTO gmDto) return (TDestination)(object)ToEntity(gmDto);
        if (typeof(TDestination) == typeof(GrupoModificadorDTO) && source is GrupoModificador gm) return (TDestination)(object)ToDTO(gm);
        if (typeof(TDestination) == typeof(IEnumerable<GrupoModificadorDTO>) && source is IEnumerable<GrupoModificador> gmList) return (TDestination)(object)ToDTOList(gmList);
        if (typeof(TDestination) == typeof(List<GrupoModificadorDTO>) && source is IEnumerable<GrupoModificador> gmList2) return (TDestination)(object)ToDTOList(gmList2);

        // Menu
        if (typeof(TDestination) == typeof(Menu) && source is MenuDTO mDto) return (TDestination)(object)ToEntity(mDto);
        if (typeof(TDestination) == typeof(MenuDTO) && source is Menu m) return (TDestination)(object)ToDTO(m);
        if (typeof(TDestination) == typeof(IEnumerable<MenuDTO>) && source is IEnumerable<Menu> mList) return (TDestination)(object)ToDTOList(mList);
        if (typeof(TDestination) == typeof(List<MenuDTO>) && source is IEnumerable<Menu> mList2) return (TDestination)(object)ToDTOList(mList2);

        // Mesa
        if (typeof(TDestination) == typeof(Mesa) && source is MesaDTO mesaDto) return (TDestination)(object)ToEntity(mesaDto);
        if (typeof(TDestination) == typeof(MesaDTO) && source is Mesa mesa) return (TDestination)(object)ToDTO(mesa);
        if (typeof(TDestination) == typeof(IEnumerable<MesaDTO>) && source is IEnumerable<Mesa> mesaList) return (TDestination)(object)ToDTOList(mesaList);
        if (typeof(TDestination) == typeof(List<MesaDTO>) && source is IEnumerable<Mesa> mesaList2) return (TDestination)(object)ToDTOList(mesaList2);

        // MovimientoCaja
        if (typeof(TDestination) == typeof(MovimientoCaja) && source is MovimientoCajaDTO mcDto) return (TDestination)(object)ToEntity(mcDto);
        if (typeof(TDestination) == typeof(MovimientoCajaDTO) && source is MovimientoCaja mc) return (TDestination)(object)ToDTO(mc);
        if (typeof(TDestination) == typeof(IEnumerable<MovimientoCajaDTO>) && source is IEnumerable<MovimientoCaja> mcList) return (TDestination)(object)ToDTOList(mcList);
        if (typeof(TDestination) == typeof(List<MovimientoCajaDTO>) && source is IEnumerable<MovimientoCaja> mcList2) return (TDestination)(object)ToDTOList(mcList2);

        // OpcionModificador
        if (typeof(TDestination) == typeof(OpcionModificador) && source is OpcionModificadorDTO omDto) return (TDestination)(object)ToEntity(omDto);
        if (typeof(TDestination) == typeof(OpcionModificadorDTO) && source is OpcionModificador om) return (TDestination)(object)ToDTO(om);
        if (typeof(TDestination) == typeof(IEnumerable<OpcionModificadorDTO>) && source is IEnumerable<OpcionModificador> omList) return (TDestination)(object)ToDTOList(omList);
        if (typeof(TDestination) == typeof(List<OpcionModificadorDTO>) && source is IEnumerable<OpcionModificador> omList2) return (TDestination)(object)ToDTOList(omList2);

        // Pago
        if (typeof(TDestination) == typeof(Pago) && source is PagoDTO pDto) return (TDestination)(object)ToEntity(pDto);
        if (typeof(TDestination) == typeof(PagoDTO) && source is Pago p) return (TDestination)(object)ToDTO(p);
        if (typeof(TDestination) == typeof(IEnumerable<PagoDTO>) && source is IEnumerable<Pago> pList) return (TDestination)(object)ToDTOList(pList);
        if (typeof(TDestination) == typeof(List<PagoDTO>) && source is IEnumerable<Pago> pList2) return (TDestination)(object)ToDTOList(pList2);

        // Pedido
        if (typeof(TDestination) == typeof(Pedido) && source is PedidoDTO pedDto) return (TDestination)(object)ToEntity(pedDto);
        if (typeof(TDestination) == typeof(PedidoDTO) && source is Pedido ped) return (TDestination)(object)ToDTO(ped);
        if (typeof(TDestination) == typeof(IEnumerable<PedidoDTO>) && source is IEnumerable<Pedido> pedList) return (TDestination)(object)ToDTOList(pedList);
        if (typeof(TDestination) == typeof(List<PedidoDTO>) && source is IEnumerable<Pedido> pedList2) return (TDestination)(object)ToDTOList(pedList2);
        if (typeof(TDestination) == typeof(Pedido) && source is CrearPedidoRequestDTO cprDto) return (TDestination)(object)ToEntity(cprDto);
        if (typeof(TDestination) == typeof(CrearPedidoRequestDTO) && source is Pedido cprPed) return (TDestination)(object)ToCrearPedidoRequestDTO(cprPed);

        // PedidoAsiento
        if (typeof(TDestination) == typeof(PedidoAsiento) && source is PedidoAsientoDTO paDto) return (TDestination)(object)ToEntity(paDto);
        if (typeof(TDestination) == typeof(PedidoAsientoDTO) && source is PedidoAsiento pa) return (TDestination)(object)ToDTO(pa);
        if (typeof(TDestination) == typeof(IEnumerable<PedidoAsientoDTO>) && source is IEnumerable<PedidoAsiento> paList) return (TDestination)(object)ToDTOList(paList);
        if (typeof(TDestination) == typeof(List<PedidoAsientoDTO>) && source is IEnumerable<PedidoAsiento> paList2) return (TDestination)(object)ToDTOList(paList2);

        // PedidoDetalle
        if (typeof(TDestination) == typeof(PedidoDetalle) && source is PedidoDetalleDTO pdDto) return (TDestination)(object)ToEntity(pdDto);
        if (typeof(TDestination) == typeof(PedidoDetalleDTO) && source is PedidoDetalle pd) return (TDestination)(object)ToDTO(pd);
        if (typeof(TDestination) == typeof(IEnumerable<PedidoDetalleDTO>) && source is IEnumerable<PedidoDetalle> pdList) return (TDestination)(object)ToDTOList(pdList);
        if (typeof(TDestination) == typeof(List<PedidoDetalleDTO>) && source is IEnumerable<PedidoDetalle> pdList2) return (TDestination)(object)ToDTOList(pdList2);
        if (typeof(TDestination) == typeof(PedidoDetalle) && source is CrearPedidoDetalleDTO cpdDto) return (TDestination)(object)ToEntity(cpdDto);
        if (typeof(TDestination) == typeof(CrearPedidoDetalleDTO) && source is PedidoDetalle cpdPd) return (TDestination)(object)ToCrearPedidoDetalleDTO(cpdPd);

        // PedidoModificador
        if (typeof(TDestination) == typeof(PedidoModificador) && source is PedidoModificadorDTO pmDto) return (TDestination)(object)ToEntity(pmDto);
        if (typeof(TDestination) == typeof(PedidoModificadorDTO) && source is PedidoModificador pm) return (TDestination)(object)ToDTO(pm);
        if (typeof(TDestination) == typeof(IEnumerable<PedidoModificadorDTO>) && source is IEnumerable<PedidoModificador> pmList) return (TDestination)(object)ToDTOList(pmList);
        if (typeof(TDestination) == typeof(List<PedidoModificadorDTO>) && source is IEnumerable<PedidoModificador> pmList2) return (TDestination)(object)ToDTOList(pmList2);

        // Precio
        if (typeof(TDestination) == typeof(Precio) && source is PrecioDTO prDto) return (TDestination)(object)ToEntity(prDto);
        if (typeof(TDestination) == typeof(PrecioDTO) && source is Precio pr) return (TDestination)(object)ToDTO(pr);
        if (typeof(TDestination) == typeof(IEnumerable<PrecioDTO>) && source is IEnumerable<Precio> prList) return (TDestination)(object)ToDTOList(prList);
        if (typeof(TDestination) == typeof(List<PrecioDTO>) && source is IEnumerable<Precio> prList2) return (TDestination)(object)ToDTOList(prList2);

        // Producto
        if (typeof(TDestination) == typeof(Producto) && source is ProductoDTO prodDto) return (TDestination)(object)ToEntity(prodDto);
        if (typeof(TDestination) == typeof(ProductoDTO) && source is Producto prod) return (TDestination)(object)ToDTO(prod);
        if (typeof(TDestination) == typeof(IEnumerable<ProductoDTO>) && source is IEnumerable<Producto> prodList) return (TDestination)(object)ToDTOList(prodList);
        if (typeof(TDestination) == typeof(List<ProductoDTO>) && source is IEnumerable<Producto> prodList2) return (TDestination)(object)ToDTOList(prodList2);

        // Rol
        if (typeof(TDestination) == typeof(Rol) && source is RolDTO rDto) return (TDestination)(object)ToEntity(rDto);
        if (typeof(TDestination) == typeof(RolDTO) && source is Rol r) return (TDestination)(object)ToDTO(r);
        if (typeof(TDestination) == typeof(IEnumerable<RolDTO>) && source is IEnumerable<Rol> rList) return (TDestination)(object)ToDTOList(rList);
        if (typeof(TDestination) == typeof(List<RolDTO>) && source is IEnumerable<Rol> rList2) return (TDestination)(object)ToDTOList(rList2);

        // RolAccesoRuta
        if (typeof(TDestination) == typeof(RolAccesoRuta) && source is RolAccesoRutaDTO rarDto) return (TDestination)(object)ToEntity(rarDto);
        if (typeof(TDestination) == typeof(RolAccesoRutaDTO) && source is RolAccesoRuta rar) return (TDestination)(object)ToDTO(rar);
        if (typeof(TDestination) == typeof(IEnumerable<RolAccesoRutaDTO>) && source is IEnumerable<RolAccesoRuta> rarList) return (TDestination)(object)ToDTOList(rarList);
        if (typeof(TDestination) == typeof(List<RolAccesoRutaDTO>) && source is IEnumerable<RolAccesoRuta> rarList2) return (TDestination)(object)ToDTOList(rarList2);

        // Sucursal
        if (typeof(TDestination) == typeof(Sucursal) && source is SucursalDTO sDto) return (TDestination)(object)ToEntity(sDto);
        if (typeof(TDestination) == typeof(SucursalDTO) && source is Sucursal s) return (TDestination)(object)ToDTO(s);
        if (typeof(TDestination) == typeof(IEnumerable<SucursalDTO>) && source is IEnumerable<Sucursal> sList) return (TDestination)(object)ToDTOList(sList);
        if (typeof(TDestination) == typeof(List<SucursalDTO>) && source is IEnumerable<Sucursal> sList2) return (TDestination)(object)ToDTOList(sList2);

        // TicketCocina
        if (typeof(TDestination) == typeof(TicketCocina) && source is TicketCocinaDTO tcDto) return (TDestination)(object)ToEntity(tcDto);
        if (typeof(TDestination) == typeof(TicketCocinaDTO) && source is TicketCocina tc) return (TDestination)(object)ToDTO(tc);
        if (typeof(TDestination) == typeof(IEnumerable<TicketCocinaDTO>) && source is IEnumerable<TicketCocina> tcList) return (TDestination)(object)ToDTOList(tcList);
        if (typeof(TDestination) == typeof(List<TicketCocinaDTO>) && source is IEnumerable<TicketCocina> tcList2) return (TDestination)(object)ToDTOList(tcList2);

        // TicketDetalle
        if (typeof(TDestination) == typeof(TicketDetalle) && source is TicketDetalleDTO tdDto) return (TDestination)(object)ToEntity(tdDto);
        if (typeof(TDestination) == typeof(TicketDetalleDTO) && source is TicketDetalle td) return (TDestination)(object)ToDTO(td);
        if (typeof(TDestination) == typeof(IEnumerable<TicketDetalleDTO>) && source is IEnumerable<TicketDetalle> tdList) return (TDestination)(object)ToDTOList(tdList);
        if (typeof(TDestination) == typeof(List<TicketDetalleDTO>) && source is IEnumerable<TicketDetalle> tdList2) return (TDestination)(object)ToDTOList(tdList2);

        // Turno
        if (typeof(TDestination) == typeof(Turno) && source is TurnoDTO tDto) return (TDestination)(object)ToEntity(tDto);
        if (typeof(TDestination) == typeof(TurnoDTO) && source is Turno t) return (TDestination)(object)ToDTO(t);
        if (typeof(TDestination) == typeof(IEnumerable<TurnoDTO>) && source is IEnumerable<Turno> tList) return (TDestination)(object)ToDTOList(tList);
        if (typeof(TDestination) == typeof(List<TurnoDTO>) && source is IEnumerable<Turno> tList2) return (TDestination)(object)ToDTOList(tList2);

        // Usuario
        if (typeof(TDestination) == typeof(Usuario) && source is UsuarioDTO uDto) return (TDestination)(object)ToEntity(uDto);
        if (typeof(TDestination) == typeof(UsuarioDTO) && source is Usuario u) return (TDestination)(object)ToDTO(u);
        if (typeof(TDestination) == typeof(IEnumerable<UsuarioDTO>) && source is IEnumerable<Usuario> uList) return (TDestination)(object)ToDTOList(uList);
        if (typeof(TDestination) == typeof(List<UsuarioDTO>) && source is IEnumerable<Usuario> uList2) return (TDestination)(object)ToDTOList(uList2);

        // UsuarioRol
        if (typeof(TDestination) == typeof(UsuarioRol) && source is UsuarioRolDTO urDto) return (TDestination)(object)ToEntity(urDto);
        if (typeof(TDestination) == typeof(UsuarioRolDTO) && source is UsuarioRol ur) return (TDestination)(object)ToDTO(ur);
        if (typeof(TDestination) == typeof(IEnumerable<UsuarioRolDTO>) && source is IEnumerable<UsuarioRol> urList) return (TDestination)(object)ToDTOList(urList);
        if (typeof(TDestination) == typeof(List<UsuarioRolDTO>) && source is IEnumerable<UsuarioRol> urList2) return (TDestination)(object)ToDTOList(urList2);

        // VarianteProducto
        if (typeof(TDestination) == typeof(VarianteProducto) && source is VarianteProductoDTO vpDto) return (TDestination)(object)ToEntity(vpDto);
        if (typeof(TDestination) == typeof(VarianteProductoDTO) && source is VarianteProducto vp) return (TDestination)(object)ToDTO(vp);
        if (typeof(TDestination) == typeof(IEnumerable<VarianteProductoDTO>) && source is IEnumerable<VarianteProducto> vpList) return (TDestination)(object)ToDTOList(vpList);
        if (typeof(TDestination) == typeof(List<VarianteProductoDTO>) && source is IEnumerable<VarianteProducto> vpList2) return (TDestination)(object)ToDTOList(vpList2);

        // Generic Catalog
        if (typeof(TDestination) == typeof(GenericCatalogDTO) && source is ICatalogEntity catEnt) return (TDestination)(object)ToGenericCatalogDTO(catEnt);
        if (typeof(TDestination) == typeof(IEnumerable<GenericCatalogDTO>) && source is IEnumerable<ICatalogEntity> catList) return (TDestination)(object)ToGenericCatalogDTOList(catList);
        if (typeof(TDestination) == typeof(List<GenericCatalogDTO>) && source is IEnumerable<ICatalogEntity> catList2) return (TDestination)(object)ToGenericCatalogDTOList(catList2);

        if (typeof(ICatalogEntity).IsAssignableFrom(typeof(TDestination)) && typeof(BaseAuditableEntity).IsAssignableFrom(typeof(TDestination)) && source is GenericCatalogDTO gcDto)
        {
            var destEntity = (BaseAuditableEntity)Activator.CreateInstance(typeof(TDestination))!;
            destEntity.Id = gcDto.Id;
            destEntity.IsActive = gcDto.IsActive;
            if (destEntity is ICatalogEntity ce)
            {
                ce.Descripcion = gcDto.Descripcion;
            }
            var pCodigo = typeof(TDestination).GetProperty("Codigo");
            if (pCodigo != null && pCodigo.CanWrite)
            {
                pCodigo.SetValue(destEntity, gcDto.Codigo);
            }
            return (TDestination)(object)destEntity;
        }

        throw new NotSupportedException($"No mapping defined between {source.GetType().Name} and {typeof(TDestination).Name}");
    }
}
