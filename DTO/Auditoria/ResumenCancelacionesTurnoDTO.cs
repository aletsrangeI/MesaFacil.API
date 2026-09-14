namespace DTO.Auditoria;

/// <summary>Spec 024, sección 3 y 2.4: KPIs y desglose de cancelaciones del turno.</summary>
public class ResumenCancelacionesTurnoDTO
{
    public decimal TotalVentasTurno { get; set; }
    public decimal TotalCancelacionesTurno { get; set; }
    public decimal PorcentajeCancelaciones { get; set; }
    public bool SuperaUmbralAlerta { get; set; }
    public int TotalEventos { get; set; }
    public List<DesgloseCancelacionDTO> Desglose { get; set; } = new();
}

public class DesgloseCancelacionDTO
{
    public DateTime FechaHora { get; set; }
    public string? Mesa { get; set; }
    public string? Platillo { get; set; }
    public decimal Importe { get; set; }
    public string? Mesero { get; set; }
    public string? Supervisor { get; set; }
    public string? Motivo { get; set; }
}
