using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Mesa;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Mesas;

public class MesaApplication : IMesaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MesaDTOValidator _validationRules;
    private readonly IAppLogger<MesaApplication> _logger;

    public MesaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        MesaDTOValidator validationRules,
        IAppLogger<MesaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = _unitOfWork.Mesas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = _unitOfWork.Mesas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Delete(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = _unitOfWork.Mesas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<MesaDTO> Get(int id)
    {
        var response = new Response<MesaDTO>();
        try
        {
            var entity = _unitOfWork.Mesas.Get(id);
            response.Data = _mapper.Map<MesaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<MesaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<MesaDTO>>();
        try
        {
            var list = _unitOfWork.Mesas.GetAll();
            response.Data = EnrichMesasDTO(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<MesaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MesaDTO>>();
        try
        {
            var list = _unitOfWork.Mesas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MesaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<int> Count()
    {
        var response = new Response<int>();
        try
        {
            response.Data = _unitOfWork.Mesas.Count();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> SolicitarCuenta(int idMesa)
    {
        var response = new Response<bool>();
        try
        {
            var mesa = _unitOfWork.Mesas.Get(idMesa);
            if (mesa == null)
            {
                response.isSuccess = false;
                response.Message = "Mesa no encontrada";
                return response;
            }

            mesa.IdEstadoMesa = EstadosMesaConst.PidiendoCuenta;
            mesa.UpdatedAt = DateTime.UtcNow;
            mesa.UpdatedBy = "Mesero";
            response.Data = _unitOfWork.Mesas.Update(mesa);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta solicitada correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = await _unitOfWork.Mesas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = await _unitOfWork.Mesas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.Mesas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<MesaDTO>> GetAsync(int id)
    {
        var response = new Response<MesaDTO>();
        try
        {
            var entity = await _unitOfWork.Mesas.GetAsync(id);
            response.Data = _mapper.Map<MesaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<MesaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<MesaDTO>>();
        try
        {
            var list = await _unitOfWork.Mesas.GetAllAsync();
            response.Data = EnrichMesasDTO(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<MesaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MesaDTO>>();
        try
        {
            var list = await _unitOfWork.Mesas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = EnrichMesasDTO(list);
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<int>> CountAsync()
    {
        var response = new Response<int>();
        try
        {
            response.Data = await _unitOfWork.Mesas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> SolicitarCuentaAsync(int idMesa)
    {
        var response = new Response<bool>();
        try
        {
            var mesa = await _unitOfWork.Mesas.GetAsync(idMesa);
            if (mesa == null)
            {
                response.isSuccess = false;
                response.Message = "Mesa no encontrada";
                return response;
            }

            mesa.IdEstadoMesa = EstadosMesaConst.PidiendoCuenta;
            mesa.UpdatedAt = DateTime.UtcNow;
            mesa.UpdatedBy = "Mesero";
            response.Data = await _unitOfWork.Mesas.UpdateAsync(mesa);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta solicitada correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UnirMesasAsync(UnirMesasDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            if (dto.IdMesaPrincipal <= 0 || dto.IdsMesasSecundarias == null || !dto.IdsMesasSecundarias.Any())
            {
                response.isSuccess = false;
                response.Message = "Debe especificar la mesa principal y al menos una mesa secundaria.";
                return response;
            }

            var mesaPrincipal = await _unitOfWork.Mesas.GetAsync(dto.IdMesaPrincipal);
            if (mesaPrincipal == null)
            {
                response.isSuccess = false;
                response.Message = $"No se encontró la mesa principal con ID {dto.IdMesaPrincipal}.";
                return response;
            }

            // Si la mesa principal seleccionada ya está unida a otra, usar la verdadera mesa principal
            int targetPrincipalId = mesaPrincipal.IdMesaPrincipal ?? mesaPrincipal.Id;
            if (targetPrincipalId != mesaPrincipal.Id)
            {
                var realPrincipal = await _unitOfWork.Mesas.GetAsync(targetPrincipalId);
                if (realPrincipal != null)
                {
                    mesaPrincipal = realPrincipal;
                }
            }

            var todasMesas = (await _unitOfWork.Mesas.GetAllAsync()).ToList();

            foreach (var idSecundaria in dto.IdsMesasSecundarias)
            {
                if (idSecundaria == mesaPrincipal.Id) continue;

                var mesaSec = todasMesas.FirstOrDefault(m => m.Id == idSecundaria);
                if (mesaSec == null) continue;

                mesaSec.IdMesaPrincipal = mesaPrincipal.Id;
                // Sincronizar estado con la principal (si está ocupada o pidiendo cuenta)
                if (mesaPrincipal.IdEstadoMesa == EstadosMesaConst.Ocupada || mesaPrincipal.IdEstadoMesa == EstadosMesaConst.PidiendoCuenta)
                {
                    mesaSec.IdEstadoMesa = mesaPrincipal.IdEstadoMesa;
                }
                else
                {
                    // Si ambas estaban libres, pasarlas a Ocupadas
                    mesaPrincipal.IdEstadoMesa = EstadosMesaConst.Ocupada;
                    mesaSec.IdEstadoMesa = EstadosMesaConst.Ocupada;
                }

                mesaSec.UpdatedAt = DateTime.UtcNow;
                mesaSec.UpdatedBy = "Sistema";
                await _unitOfWork.Mesas.UpdateAsync(mesaSec);
            }

            mesaPrincipal.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Mesas.UpdateAsync(mesaPrincipal);

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Mesas unidas con éxito.";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> DesunirMesaAsync(int idMesa)
    {
        var response = new Response<bool>();
        try
        {
            var mesa = await _unitOfWork.Mesas.GetAsync(idMesa);
            if (mesa == null)
            {
                response.isSuccess = false;
                response.Message = "Mesa no encontrada.";
                return response;
            }

            if (!mesa.IdMesaPrincipal.HasValue)
            {
                // Es mesa principal: desunir todo el grupo de mesas secundarias anexadas a ella
                return await DesunirGrupoAsync(idMesa);
            }

            // Es mesa secundaria: desunir solo esta mesa
            mesa.IdMesaPrincipal = null;
            mesa.IdEstadoMesa = EstadosMesaConst.Disponible;
            mesa.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Mesas.UpdateAsync(mesa);

            response.Data = true;
            response.isSuccess = true;
            response.Message = $"Mesa {mesa.Codigo} desunida con éxito.";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> DesunirGrupoAsync(int idMesaPrincipal)
    {
        var response = new Response<bool>();
        try
        {
            var todasMesas = await _unitOfWork.Mesas.GetAllAsync();
            var secundarias = todasMesas.Where(m => m.IdMesaPrincipal == idMesaPrincipal).ToList();

            foreach (var sec in secundarias)
            {
                sec.IdMesaPrincipal = null;
                sec.IdEstadoMesa = EstadosMesaConst.Disponible;
                sec.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Mesas.UpdateAsync(sec);
            }

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Grupo de mesas desunido con éxito.";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    private IEnumerable<MesaDTO> EnrichMesasDTO(IEnumerable<Mesa> entities)
    {
        var entityList = entities.ToList();
        var dtos = _mapper.Map<List<MesaDTO>>(entityList);
        var mapById = dtos.ToDictionary(d => d.Id);

        foreach (var dto in dtos)
        {
            if (dto.IdMesaPrincipal.HasValue && mapById.TryGetValue(dto.IdMesaPrincipal.Value, out var principal))
            {
                dto.CodigoMesaPrincipal = principal.Codigo;
            }

            var secundarias = dtos.Where(d => d.IdMesaPrincipal == dto.Id).ToList();
            if (secundarias.Any())
            {
                dto.IdsMesasUnidas = secundarias.Select(s => s.Id).ToList();
                dto.CodigosMesasUnidas = secundarias.Select(s => s.Codigo).ToList();
                dto.AsientosTotalesGrupo = dto.Asientos + secundarias.Sum(s => s.Asientos);
            }
            else
            {
                dto.AsientosTotalesGrupo = dto.Asientos;
            }
        }

        return dtos;
    }

    #endregion
}