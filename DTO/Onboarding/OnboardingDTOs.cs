namespace DTO.Onboarding;

public class ProvisionarRestauranteRequestDTO
{
    public DatosEmpresaDTO DatosEmpresa { get; set; } = new();
    public List<EstacionCocinaOnboardingDTO> EstacionesCocina { get; set; } = new();
    public List<AreaYMesasOnboardingDTO> AreasYMesas { get; set; } = new();
    public MenuOnboardingDTO Menu { get; set; } = new();
    public List<PersonalOnboardingDTO> Personal { get; set; } = new();
    public string? PinSupervisorAdmin { get; set; }
    public bool AbrirTurnoInicial { get; set; } = true;
    public decimal FondoCajaInicial { get; set; } = 1000.00m;
}

public class DatosEmpresaDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string NombreSucursal { get; set; } = "Sucursal Matriz";
    public string? Direccion { get; set; }
    public string ZonaHoraria { get; set; } = "America/Mexico_City";
    public string Moneda { get; set; } = "MXN";
    public decimal TasaIva { get; set; } = 16.0m;
}

public class EstacionCocinaOnboardingDTO
{
    public string Nombre { get; set; } = string.Empty;
    public int MinutosAmbar { get; set; } = 7;
    public int MinutosRojo { get; set; } = 12;
}

public class AreaYMesasOnboardingDTO
{
    public string NombreArea { get; set; } = string.Empty;
    public int Orden { get; set; } = 1;
    public string PrefijoMesa { get; set; } = "M";
    public int CantidadMesas { get; set; } = 6;
    public int AsientosPorMesa { get; set; } = 4;
}

public class MenuOnboardingDTO
{
    public string NombreMenu { get; set; } = "Menú Principal";
    public List<string> Categorias { get; set; } = new();
    public List<ProductoOnboardingDTO> Productos { get; set; } = new();
    public List<GrupoModificadorOnboardingDTO> GruposModificador { get; set; } = new();
}

public class ProductoOnboardingDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreCategoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? EstacionCocina { get; set; }
    public string? Descripcion { get; set; }
}

public class GrupoModificadorOnboardingDTO
{
    public string NombreProducto { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public bool Obligatorio { get; set; } = false;
    public int MinSeleccion { get; set; } = 0;
    public int MaxSeleccion { get; set; } = 1;
    public List<OpcionModificadorOnboardingDTO> Opciones { get; set; } = new();
}

public class OpcionModificadorOnboardingDTO
{
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioExtra { get; set; } = 0m;
    public bool EsDefault { get; set; } = false;
}

public class PersonalOnboardingDTO
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = "Mesero"; // "Mesero", "Manager", "Repartidor"
    public string Pin { get; set; } = string.Empty; // 4 dígitos
    public string? Telefono { get; set; } // Opcional, para repartidores o contacto
}

public class ProvisionarRestauranteResponseDTO
{
    public int EmpresaId { get; set; }
    public int SucursalId { get; set; }
    public int MesasCreadas { get; set; }
    public int ProductosCreados { get; set; }
    public int UsuariosCreados { get; set; }
    public int? TurnoId { get; set; }
    public string RutaRedirect { get; set; } = "/ventas/pos";
}

public class OnboardingEstadoDTO
{
    public bool OnboardingCompletado { get; set; }
    public bool TieneSucursales { get; set; }
    public int TotalMesas { get; set; }
    public int TotalProductos { get; set; }
    public bool TieneTurnoAbierto { get; set; }
    public int PasoSugerido { get; set; } = 1;
}
