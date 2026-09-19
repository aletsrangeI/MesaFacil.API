namespace DTO.Mesa;

public class MesaDTO
{
    public int Id { get; set; }
    public int IdSucursal { get; set; }
    public int? IdArea { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int Asientos { get; set; }
    public int IdEstadoMesa { get; set; }

    // Fusión / Agrupación de Mesas
    public int? IdMesaPrincipal { get; set; }
    public string? CodigoMesaPrincipal { get; set; }
    public List<int> IdsMesasUnidas { get; set; } = new();
    public List<string> CodigosMesasUnidas { get; set; } = new();
    public int AsientosTotalesGrupo { get; set; }
}

public class UnirMesasDTO
{
    public int IdMesaPrincipal { get; set; }
    public List<int> IdsMesasSecundarias { get; set; } = new();
}

public class DesunirMesaDTO
{
    public int IdMesa { get; set; }
}
