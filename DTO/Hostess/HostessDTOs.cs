namespace DTO.Hostess;

public class FilaEsperaItemDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public string Telefono => TelefonoCliente;
    public int NumeroPersonas { get; set; }
    public int Comensales => NumeroPersonas;
    public string? ZonaPreferencia { get; set; }
    public int MinutosEstimados { get; set; }
    public int TiempoEsperaEstimadoMinutos => MinutosEstimados;
    public int MinutosTranscurridos { get; set; }
    public DateTime RegistradoEn { get; set; }
    public DateTime FechaLlegada => RegistradoEn;
    public DateTime? NotificadoEn { get; set; }
    public string Estado { get; set; } = "EnEspera"; // EnEspera, Notificado, Sentado, Cancelado
    public int? IdMesaAsignada { get; set; }
    public string? CodigoMesaAsignada { get; set; }
    public string? EnlaceWhatsApp { get; set; }
    public string? UrlWhatsApp => EnlaceWhatsApp;
}

public class RegistrarWaitlistDTO
{
    public int IdSucursal { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public string? Telefono
    {
        get => TelefonoCliente;
        set => TelefonoCliente = value ?? string.Empty;
    }
    public int NumeroPersonas { get; set; } = 2;
    public int? Comensales
    {
        get => NumeroPersonas;
        set { if (value.HasValue && value.Value > 0) NumeroPersonas = value.Value; }
    }
    public string? ZonaPreferencia { get; set; }
    public string? Notas { get; set; }
    public int? MinutosEstimadosPersonalizados { get; set; }
}

public class SentarWaitlistDTO
{
    public int IdMesa { get; set; }
}

public class ReservaMesaDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdMesaAsignada => IdMesa;
    public string? CodigoMesa { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public string Telefono => TelefonoCliente;
    public string? Correo { get; set; }
    public DateTime FechaHoraReserva { get; set; }
    public int NumeroPersonas { get; set; }
    public int Comensales => NumeroPersonas;
    public string? ZonaPreferencia { get; set; }
    public string EstadoReserva { get; set; } = "Confirmada"; // Confirmada, Sentada, Cancelada, NoShow
    public string Estado => EstadoReserva;
    public decimal AnticipoPagado { get; set; }
    public decimal DepositoGarantia => AnticipoPagado;
    public bool DepositoPagado => AnticipoPagado > 0;
    public string? Notas { get; set; }
}

public class CrearReservaDTO
{
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdMesaAsignada
    {
        get => IdMesa;
        set => IdMesa = value;
    }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public string? Telefono
    {
        get => TelefonoCliente;
        set => TelefonoCliente = value ?? string.Empty;
    }
    public string? Correo { get; set; }
    public DateTime FechaHoraReserva { get; set; }
    public int NumeroPersonas { get; set; } = 2;
    public int? Comensales
    {
        get => NumeroPersonas;
        set { if (value.HasValue && value.Value > 0) NumeroPersonas = value.Value; }
    }
    public string? ZonaPreferencia { get; set; }
    public decimal AnticipoPagado { get; set; } = 0m;
    public decimal? DepositoGarantia
    {
        get => AnticipoPagado;
        set { if (value.HasValue) AnticipoPagado = value.Value; }
    }
    public bool? DepositoPagado { get; set; }
    public string? Notas { get; set; }
}

public class ConfirmarLlegadaReservaDTO
{
    public int? IdMesa { get; set; }
}

public class HostessDashboardSummaryDTO
{
    public int TotalEnEspera { get; set; }
    public int TotalNotificados { get; set; }
    public int MinutosPromedioEspera { get; set; }
    public int MesasDisponibles { get; set; }
    public int MesasPidiendoCuenta { get; set; }
    public int ReservasHoy { get; set; }
    public List<ReservaMesaDTO> ReservasProximas { get; set; } = new();
}
