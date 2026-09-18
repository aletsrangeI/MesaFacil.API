namespace DTO.Edge;

public class EdgeStatusDTO
{
    public string Profile { get; set; } = string.Empty;
    public string DatabaseProvider { get; set; } = string.Empty;
    public bool IsSqlite { get; set; }
    public string CloudBaseUrl { get; set; } = string.Empty;
    public EdgeOutboxStatusDTO Outbox { get; set; } = new();
    public EdgeNetworkStatusDTO Network { get; set; } = new();
    public List<EdgeBackupItemDTO> RecentBackups { get; set; } = new();
    public DateTime ServerUtc { get; set; }
}

public class EdgeOutboxStatusDTO
{
    public int Pending { get; set; }
    public int Total { get; set; }
}

public class EdgeNetworkStatusDTO
{
    public string MachineName { get; set; } = string.Empty;
    public List<string> LanAddresses { get; set; } = new();
    public int ServicePort { get; set; }
}

public class EdgeBackupItemDTO
{
    public string Name { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class EdgeBackupResultDTO
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
