using Domain.Entities;
using DTO.Hostess;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Hostess;

public class HostessService : IHostessService
{
    private readonly ApplicationDbContext _context;

    public HostessService(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Waitlist (Fila de Espera)

    public async Task<IEnumerable<FilaEsperaItemDTO>> GetWaitlistAsync(int idSucursal)
    {
        var ahora = DateTime.UtcNow;
        var items = await _context.FilaEsperaItems
            .Include(f => f.MesaAsignada)
            .Include(f => f.Sucursal)
            .Where(f => f.IdSucursal == idSucursal && f.IsActive && (f.Estado == "EnEspera" || f.Estado == "Notificado"))
            .OrderBy(f => f.RegistradoEn)
            .ToListAsync();

        return items.Select(f => MapFilaItemToDTO(f, ahora)).ToList();
    }

    public async Task<FilaEsperaItemDTO> RegistrarEnWaitlistAsync(RegistrarWaitlistDTO dto)
    {
        int minutosEstimados = dto.MinutosEstimadosPersonalizados.HasValue && dto.MinutosEstimadosPersonalizados.Value > 0
            ? dto.MinutosEstimadosPersonalizados.Value
            : await CalcularMinutosEstimadosAsync(dto.IdSucursal, dto.NumeroPersonas, dto.ZonaPreferencia);

        // Obtener IdEmpresa a partir de la sucursal
        var sucursal = await _context.Sucursales.FindAsync(dto.IdSucursal);
        int idEmpresa = sucursal?.IdEmpresa ?? 1;

        var entity = new FilaEsperaItem
        {
            IdEmpresa = idEmpresa,
            IdSucursal = dto.IdSucursal,
            NombreCliente = dto.NombreCliente.Trim(),
            TelefonoCliente = dto.TelefonoCliente.Trim(),
            NumeroPersonas = dto.NumeroPersonas > 0 ? dto.NumeroPersonas : 2,
            ZonaPreferencia = dto.ZonaPreferencia,
            MinutosEstimados = minutosEstimados,
            RegistradoEn = DateTime.UtcNow,
            Estado = "EnEspera",
            IsActive = true
        };

        _context.FilaEsperaItems.Add(entity);
        await _context.SaveChangesAsync();

        return MapFilaItemToDTO(entity, DateTime.UtcNow, sucursal?.Nombre);
    }

    public async Task<FilaEsperaItemDTO> NotificarWaitlistAsync(int id)
    {
        var entity = await _context.FilaEsperaItems
            .Include(f => f.MesaAsignada)
            .Include(f => f.Sucursal)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (entity == null)
            throw new KeyNotFoundException($"No se encontró el registro de espera con ID {id}.");

        entity.Estado = "Notificado";
        entity.NotificadoEn = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapFilaItemToDTO(entity, DateTime.UtcNow, entity.Sucursal?.Nombre);
    }

    public async Task<bool> SentarWaitlistAsync(int id, SentarWaitlistDTO dto)
    {
        var item = await _context.FilaEsperaItems.FindAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"No se encontró el registro de espera con ID {id}.");

        var mesa = await _context.Mesas.FindAsync(dto.IdMesa);
        if (mesa == null)
            throw new KeyNotFoundException($"No se encontró la mesa con ID {dto.IdMesa}.");

        // Asignar mesa y marcar comensal como sentado
        item.Estado = "Sentado";
        item.IdMesaAsignada = dto.IdMesa;
        item.UpdatedAt = DateTime.UtcNow;

        // La mesa pasa a estar ocupada en el sistema
        mesa.IdEstadoMesa = EstadosMesaConst.Ocupada;
        mesa.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelarWaitlistAsync(int id)
    {
        var item = await _context.FilaEsperaItems.FindAsync(id);
        if (item == null) return false;

        item.Estado = "Cancelado";
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CalcularMinutosEstimadosAsync(int idSucursal, int numeroPersonas, string? zonaPreferencia = null)
    {
        // 1. Obtener mesas activas de la sucursal que tengan capacidad suficiente
        var queryMesas = _context.Mesas
            .Include(m => m.Area)
            .Where(m => m.IdSucursal == idSucursal && m.IsActive && m.Asientos >= numeroPersonas);

        if (!string.IsNullOrWhiteSpace(zonaPreferencia) && zonaPreferencia != "Cualquiera" && zonaPreferencia != "Indistinto")
        {
            var zonaLower = zonaPreferencia.Trim().ToLower();
            // Si hay mesas en esa zona, filtramos por ella
            bool existenEnZona = await queryMesas.AnyAsync(m => m.Area != null && m.Area.Nombre.ToLower().Contains(zonaLower));
            if (existenEnZona)
            {
                queryMesas = queryMesas.Where(m => m.Area != null && m.Area.Nombre.ToLower().Contains(zonaLower));
            }
        }

        var mesasCompatibles = await queryMesas.ToListAsync();

        if (!mesasCompatibles.Any())
        {
            // Sin mesas del tamaño solicitado
            return 30;
        }

        // Si alguna mesa compatible está disponible ahora mismo:
        int mesasDisponibles = mesasCompatibles.Count(m => m.IdEstadoMesa == EstadosMesaConst.Disponible);
        int enEsperaDelMismoTamano = await _context.FilaEsperaItems
            .CountAsync(f => f.IdSucursal == idSucursal && f.Estado == "EnEspera" && f.NumeroPersonas >= numeroPersonas - 1 && f.NumeroPersonas <= numeroPersonas + 2);

        if (mesasDisponibles > enEsperaDelMismoTamano)
        {
            return 0; // Mesa disponible de inmediato
        }

        // Mesas próximas a desocuparse:
        // - Sucia (5): limpieza toma aprox 5 min
        // - Pidiendo Cuenta (4): cobro y salida toma aprox 10-12 min
        int mesasSucias = mesasCompatibles.Count(m => m.IdEstadoMesa == EstadosMesaConst.Sucia);
        int mesasPidiendoCuenta = mesasCompatibles.Count(m => m.IdEstadoMesa == EstadosMesaConst.PidiendoCuenta);

        if (mesasSucias > 0 && enEsperaDelMismoTamano == 0)
        {
            return 5;
        }

        if (mesasPidiendoCuenta > 0 && enEsperaDelMismoTamano == 0)
        {
            return 12;
        }

        // Si están ocupadas, cálculo por rotación y grupos en cola
        int baseMinutos = 15;
        int tiempoPorCola = enEsperaDelMismoTamano * 10;

        return Math.Max(10, baseMinutos + tiempoPorCola);
    }

    #endregion

    #region Reservaciones

    public async Task<IEnumerable<ReservaMesaDTO>> GetReservasAsync(int idSucursal, DateTime fecha)
    {
        var inicioDia = fecha.Date;
        var finDia = fecha.Date.AddDays(1).AddTicks(-1);

        var reservas = await _context.ReservasMesa
            .Include(r => r.Mesa)
            .Where(r => r.IdSucursal == idSucursal && r.IsActive && r.FechaHoraReserva >= inicioDia && r.FechaHoraReserva <= finDia)
            .OrderBy(r => r.FechaHoraReserva)
            .ToListAsync();

        return reservas.Select(MapReservaToDTO).ToList();
    }

    public async Task<ReservaMesaDTO> CrearReservaAsync(CrearReservaDTO dto)
    {
        var sucursal = await _context.Sucursales.FindAsync(dto.IdSucursal);
        int idEmpresa = sucursal?.IdEmpresa ?? 1;

        var notasFinales = dto.Notas?.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Correo))
        {
            var correoTag = $"[Email: {dto.Correo.Trim()}]";
            notasFinales = string.IsNullOrWhiteSpace(notasFinales)
                ? correoTag
                : $"{notasFinales} {correoTag}";
        }

        var telefono = !string.IsNullOrWhiteSpace(dto.TelefonoCliente) ? dto.TelefonoCliente : dto.Telefono;
        var idMesaFinal = dto.IdMesa ?? dto.IdMesaAsignada;
        var anticipo = dto.AnticipoPagado > 0 ? dto.AnticipoPagado : (dto.DepositoGarantia ?? 0m);

        var entity = new ReservaMesa
        {
            IdEmpresa = idEmpresa,
            IdSucursal = dto.IdSucursal,
            IdMesa = idMesaFinal,
            NombreCliente = dto.NombreCliente.Trim(),
            TelefonoCliente = telefono?.Trim() ?? string.Empty,
            FechaHoraReserva = dto.FechaHoraReserva,
            NumeroPersonas = dto.NumeroPersonas > 0 ? dto.NumeroPersonas : 2,
            ZonaPreferencia = dto.ZonaPreferencia,
            EstadoReserva = "Confirmada",
            AnticipoPagado = anticipo,
            Notas = notasFinales,
            IsActive = true
        };

        // Si se asignó mesa previa, podemos marcarla como Reservada si falta poco
        if (idMesaFinal.HasValue)
        {
            var mesa = await _context.Mesas.FindAsync(idMesaFinal.Value);
            if (mesa != null && (dto.FechaHoraReserva - DateTime.UtcNow).TotalMinutes <= 60)
            {
                mesa.IdEstadoMesa = EstadosMesaConst.Reservada;
            }
        }

        _context.ReservasMesa.Add(entity);
        await _context.SaveChangesAsync();

        return MapReservaToDTO(entity);
    }

    public async Task<bool> ConfirmarLlegadaReservaAsync(int id, ConfirmarLlegadaReservaDTO dto)
    {
        var reserva = await _context.ReservasMesa.FindAsync(id);
        if (reserva == null)
            throw new KeyNotFoundException($"No se encontró la reserva con ID {id}.");

        reserva.EstadoReserva = "Sentada";
        reserva.UpdatedAt = DateTime.UtcNow;

        int? idMesaFinal = dto.IdMesa ?? reserva.IdMesa;
        if (idMesaFinal.HasValue)
        {
            reserva.IdMesa = idMesaFinal.Value;
            var mesa = await _context.Mesas.FindAsync(idMesaFinal.Value);
            if (mesa != null)
            {
                mesa.IdEstadoMesa = EstadosMesaConst.Ocupada;
                mesa.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelarReservaAsync(int id, string? motivo = null)
    {
        var reserva = await _context.ReservasMesa.FindAsync(id);
        if (reserva == null) return false;

        reserva.EstadoReserva = "Cancelada";
        if (!string.IsNullOrEmpty(motivo))
        {
            reserva.Notas = string.IsNullOrEmpty(reserva.Notas)
                ? $"Cancelada: {motivo}"
                : $"{reserva.Notas} | Cancelada: {motivo}";
        }
        reserva.UpdatedAt = DateTime.UtcNow;

        // Si tenía mesa reservada y estaba en estado Reservada, liberarla a Disponible
        if (reserva.IdMesa.HasValue)
        {
            var mesa = await _context.Mesas.FindAsync(reserva.IdMesa.Value);
            if (mesa != null && mesa.IdEstadoMesa == EstadosMesaConst.Reservada)
            {
                mesa.IdEstadoMesa = EstadosMesaConst.Disponible;
                mesa.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Dashboard Resumen

    public async Task<HostessDashboardSummaryDTO> GetSummaryAsync(int idSucursal)
    {
        var hoy = DateTime.UtcNow.Date;
        var finHoy = hoy.AddDays(1).AddTicks(-1);

        var waitlistItems = await _context.FilaEsperaItems
            .Where(f => f.IdSucursal == idSucursal && f.IsActive && (f.Estado == "EnEspera" || f.Estado == "Notificado"))
            .ToListAsync();

        var mesas = await _context.Mesas
            .Where(m => m.IdSucursal == idSucursal && m.IsActive)
            .ToListAsync();

        var reservasHoy = await _context.ReservasMesa
            .Include(r => r.Mesa)
            .Where(r => r.IdSucursal == idSucursal && r.IsActive && r.FechaHoraReserva >= hoy && r.FechaHoraReserva <= finHoy)
            .OrderBy(r => r.FechaHoraReserva)
            .ToListAsync();

        int totalEspera = waitlistItems.Count(f => f.Estado == "EnEspera");
        int totalNotificados = waitlistItems.Count(f => f.Estado == "Notificado");
        int minPromedio = waitlistItems.Any()
            ? (int)Math.Round(waitlistItems.Average(f => f.MinutosEstimados))
            : 0;

        return new HostessDashboardSummaryDTO
        {
            TotalEnEspera = totalEspera,
            TotalNotificados = totalNotificados,
            MinutosPromedioEspera = minPromedio,
            MesasDisponibles = mesas.Count(m => m.IdEstadoMesa == EstadosMesaConst.Disponible),
            MesasPidiendoCuenta = mesas.Count(m => m.IdEstadoMesa == EstadosMesaConst.PidiendoCuenta),
            ReservasHoy = reservasHoy.Count,
            ReservasProximas = reservasHoy.Where(r => r.EstadoReserva == "Confirmada").Take(6).Select(MapReservaToDTO).ToList()
        };
    }

    #endregion

    #region Helpers

    private FilaEsperaItemDTO MapFilaItemToDTO(FilaEsperaItem f, DateTime ahora, string? nombreSucursal = null)
    {
        int transcurridos = Math.Max(0, (int)(ahora - f.RegistradoEn).TotalMinutes);
        string? sucursalNom = nombreSucursal ?? f.Sucursal?.Nombre;

        return new FilaEsperaItemDTO
        {
            Id = f.Id,
            IdEmpresa = f.IdEmpresa,
            IdSucursal = f.IdSucursal,
            NombreCliente = f.NombreCliente,
            TelefonoCliente = f.TelefonoCliente,
            NumeroPersonas = f.NumeroPersonas,
            ZonaPreferencia = f.ZonaPreferencia,
            MinutosEstimados = f.MinutosEstimados,
            MinutosTranscurridos = transcurridos,
            RegistradoEn = f.RegistradoEn,
            NotificadoEn = f.NotificadoEn,
            Estado = f.Estado,
            IdMesaAsignada = f.IdMesaAsignada,
            CodigoMesaAsignada = f.MesaAsignada?.Codigo,
            EnlaceWhatsApp = GenerarEnlaceWhatsApp(f.TelefonoCliente, f.NombreCliente, f.NumeroPersonas, f.ZonaPreferencia, sucursalNom)
        };
    }

    private ReservaMesaDTO MapReservaToDTO(ReservaMesa r)
    {
        string? correo = null;
        string? notasLimpia = r.Notas;
        if (!string.IsNullOrEmpty(r.Notas))
        {
            var emailTagMatch = System.Text.RegularExpressions.Regex.Match(r.Notas, @"\[Email:\s*([^\]]+)\]");
            if (emailTagMatch.Success)
            {
                correo = emailTagMatch.Groups[1].Value.Trim();
                notasLimpia = r.Notas.Replace(emailTagMatch.Value, "").Trim();
            }
        }

        return new ReservaMesaDTO
        {
            Id = r.Id,
            IdEmpresa = r.IdEmpresa,
            IdSucursal = r.IdSucursal,
            IdMesa = r.IdMesa,
            CodigoMesa = r.Mesa?.Codigo,
            NombreCliente = r.NombreCliente,
            TelefonoCliente = r.TelefonoCliente,
            Correo = correo,
            FechaHoraReserva = r.FechaHoraReserva,
            NumeroPersonas = r.NumeroPersonas,
            ZonaPreferencia = r.ZonaPreferencia,
            EstadoReserva = r.EstadoReserva,
            AnticipoPagado = r.AnticipoPagado,
            Notas = string.IsNullOrWhiteSpace(notasLimpia) ? null : notasLimpia
        };
    }

    private string? GenerarEnlaceWhatsApp(string telefono, string nombreCliente, int numeroPersonas, string? zona, string? nombreSucursal)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return null;
        var digits = new string(telefono.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits)) return null;
        if (digits.Length == 10)
        {
            // Código de país México (+52) por defecto
            digits = "52" + digits;
        }

        var sucursalText = !string.IsNullOrEmpty(nombreSucursal) ? $" en MesaFácil {nombreSucursal}" : " en MesaFácil";
        var zonaText = !string.IsNullOrEmpty(zona) && zona != "Cualquiera" && zona != "Indistinto" ? $" ({zona})" : "";
        var mensaje = $"¡Hola {nombreCliente}! Tu mesa para {numeroPersonas} personas{zonaText} ya está lista{sucursalText}. Por favor acércate con la hostess en los próximos 5 minutos para acompañarte a tu lugar. ¡Te esperamos!";

        return $"https://wa.me/{digits}?text={Uri.EscapeDataString(mensaje)}";
    }

    #endregion
}
