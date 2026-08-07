using AutoMapper;
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
using DTO.Turno;
using DTO.Usuario;
using DTO.UsuarioRol;
using DTO.VarianteProducto;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<RolAccesoRuta, RolAccesoRutaDTO>().ReverseMap();
        CreateMap<AccesoRuta, AccesoRutaDTO>().ReverseMap();
        CreateMap<FormField, FormFieldDTO>()
            .ReverseMap()
            .ForMember(dest => dest.Formulario, opt => opt.Ignore());
        CreateMap<Formulario, FormularioDTO>().ReverseMap();
        CreateMap<VarianteProducto, VarianteProductoDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<UsuarioRol, UsuarioRolDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        CreateMap<Turno, TurnoDTO>().ReverseMap();
        CreateMap<TicketDetalle, TicketDetalleDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<TicketCocina, TicketCocinaDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<Sucursal, SucursalDTO>().ReverseMap();
        CreateMap<Rol, RolDTO>().ReverseMap();
        CreateMap<Producto, ProductoDTO>().ReverseMap();
        CreateMap<Precio, PrecioDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<PedidoModificador, PedidoModificadorDTO>().ReverseMap();
        CreateMap<PedidoDetalle, PedidoDetalleDTO>().ReverseMap();
        CreateMap<PedidoAsiento, PedidoAsientoDTO>().ReverseMap();
        CreateMap<Pedido, PedidoDTO>().ReverseMap();
        
        // Mapeo Compuesto para POS
        CreateMap<CrearPedidoRequestDTO, Pedido>().ReverseMap();
        CreateMap<CrearPedidoDetalleDTO, PedidoDetalle>()
            .ForMember(dest => dest.Modificadores, opt => opt.MapFrom(src => 
                src.OpcionesModificador != null 
                ? src.OpcionesModificador.Select(id => new PedidoModificador { IdOpcion = id, IsActive = true, CreatedBy = "system" }).ToList() 
                : new List<PedidoModificador>()))
            .ReverseMap();
        
        CreateMap<Pago, PagoDTO>().ReverseMap();
        CreateMap<OpcionModificador, OpcionModificadorDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<MovimientoCaja, MovimientoCajaDTO>().ReverseMap();
        CreateMap<Mesa, MesaDTO>().ReverseMap();
        CreateMap<Menu, MenuDTO>().ReverseMap();
        CreateMap<GrupoModificador, GrupoModificadorDTO>()
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Activo));
        CreateMap<EventoPedido, EventoPedidoDTO>().ReverseMap();
        CreateMap<EstacionCocina, EstacionCocinaDTO>().ReverseMap();
        CreateMap<Empresa, EmpresaDTO>().ReverseMap();
        CreateMap<DetalleCuenta, DetalleCuentaDTO>().ReverseMap();
        CreateMap<DetalleCuenta, DetalleCuentaDTO>().ReverseMap();
        CreateMap<DetalleCuenta, DetalleCuentaDTO>().ReverseMap();
        CreateMap<DetalleCuenta, DetalleCuentaDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<DescuentoAplicado, DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Cuenta, CuentaDTO>().ReverseMap();
        CreateMap<Credencial, CredencialDTO>().ReverseMap();
        CreateMap<CorteCaja, CorteCajaDTO>().ReverseMap();
        CreateMap<Cliente, ClienteDTO>().ReverseMap();
        CreateMap<CategoriaMenu, CategoriaMenuDTO>().ReverseMap();
        CreateMap<Area, AreaDTO>().ReverseMap();
        CreateMap<CatCredencial, CatCredencialDTO>().ReverseMap();
    }
}