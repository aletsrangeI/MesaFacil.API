using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Usuario;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Usuarios;

public class UsuarioApplication : IUsuarioApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly UsuarioDTOValidator _validationRules;
    private readonly IPasswordHasher _hasher;
    private readonly IAppLogger<UsuarioApplication> _logger;

    public UsuarioApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        UsuarioDTOValidator validationRules,
        IPasswordHasher hasher,
        IAppLogger<UsuarioApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _hasher = hasher;
        _logger = logger;
    }

    private UsuarioDTO MapToUsuarioDto(Usuario user)
    {
        var dto = _mapper.Map<UsuarioDTO>(user);
        dto.NombreEmpresa = user.Empresa?.Nombre;
        dto.NombreSucursal = user.Sucursal?.Nombre;
        var ur = user.UsuarioRoles.FirstOrDefault(r => r.IsActive);
        if (ur != null)
        {
            dto.IdRol = ur.IdRol;
            dto.NombreRol = ur.Rol?.Nombre;
        }

        dto.HasPassword = user.Credenciales.Any(c => c.CatCredencial?.Descripcion == "PASSWORD" && c.IsActive);
        dto.HasPin = user.Credenciales.Any(c => c.CatCredencial?.Descripcion == "PIN" && c.IsActive);
        dto.HasPinSupervisor = !string.IsNullOrEmpty(user.PinSupervisorHash);
        dto.IsPinSupervisorLocked = user.PinBloqueadoHasta.HasValue && user.PinBloqueadoHasta.Value > DateTime.UtcNow;
        dto.PinBloqueadoHasta = user.PinBloqueadoHasta;
        dto.HasOpenTurno = user.Turnos.Any(t => t.Cierre == null && t.IsActive);

        return dto;
    }

    #region Metodos sincronos

    public Response<bool> Insert(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var validation = _validationRules.Validate(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var entity = _mapper.Map<Usuario>(dto);

            // Spec 024: Candado de Supervisor
            if (!string.IsNullOrWhiteSpace(dto.PinSupervisor))
            {
                var (supHash, supSalt) = _hasher.HashPassword(dto.PinSupervisor);
                entity.PinSupervisorHash = supHash;
                entity.PinSupervisorSalt = supSalt;
                entity.PinIntentosFallidos = 0;
                entity.PinBloqueadoHasta = null;
            }

            response.Data = _unitOfWork.Usuarios.Insert(entity);

            if (response.Data)
            {
                // 1. Asignar contraseña (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var catCred = _unitOfWork.CatCredenciales.GetAll()
                        .FirstOrDefault(c => c.Descripcion == "PASSWORD");
                    if (catCred != null)
                    {
                        var (hash, salt) = _hasher.HashPassword(dto.Password);
                        _unitOfWork.Credenciales.Insert(new Credencial
                        {
                            IdUsuario = entity.Id,
                            IdCredencial = catCred.Id,
                            Hash = hash,
                            Salt = salt,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                // 1.2 Asignar PIN de acceso rápido (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Pin))
                {
                    var catCred = _unitOfWork.CatCredenciales.GetAll()
                        .FirstOrDefault(c => c.Descripcion == "PIN");
                    if (catCred != null)
                    {
                        var (hash, salt) = _hasher.HashPassword(dto.Pin);
                        _unitOfWork.Credenciales.Insert(new Credencial
                        {
                            IdUsuario = entity.Id,
                            IdCredencial = catCred.Id,
                            Hash = hash,
                            Salt = salt,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                // 2. Asignar rol (si se proporciona)
                if (dto.IdRol.HasValue)
                {
                    _unitOfWork.UsuarioRoles.Insert(new UsuarioRol
                    {
                        UsuarioId = entity.Id,
                        IdRol = dto.IdRol.Value,
                        IsActive = true,
                        CreatedBy = "system",
                        UpdatedBy = ""
                    });
                }

                response.isSuccess = true;
                response.Message = "Usuario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var validation = _validationRules.Validate(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var existingUser = _unitOfWork.Usuarios.Get(dto.Id);
            if (existingUser == null)
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
                return response;
            }

            // Protección de turno: no desactivar usuario con turno de caja abierto
            if (!dto.IsActive && existingUser.IsActive)
            {
                var hasOpenTurno = _unitOfWork.Usuarios.HasOpenTurnoAsync(dto.Id, CancellationToken.None).GetAwaiter().GetResult();
                if (hasOpenTurno)
                {
                    response.isSuccess = false;
                    response.Message = "No es posible desactivar al usuario porque tiene un turno de caja abierto en el POS. Cierre el turno antes de continuar.";
                    return response;
                }
            }

            existingUser.NombreCompleto = dto.NombreCompleto;
            existingUser.Correo = dto.Correo;
            existingUser.IdEmpresa = dto.IdEmpresa;
            existingUser.IdSucursal = dto.IdSucursal;
            existingUser.IsActive = dto.IsActive;

            // Spec 024: Candado de Supervisor
            if (!string.IsNullOrWhiteSpace(dto.PinSupervisor))
            {
                var (supHash, supSalt) = _hasher.HashPassword(dto.PinSupervisor);
                existingUser.PinSupervisorHash = supHash;
                existingUser.PinSupervisorSalt = supSalt;
                existingUser.PinIntentosFallidos = 0;
                existingUser.PinBloqueadoHasta = null;
            }
            else if (dto.DesbloquearPinSupervisor == true)
            {
                existingUser.PinIntentosFallidos = 0;
                existingUser.PinBloqueadoHasta = null;
            }

            response.Data = _unitOfWork.Usuarios.Update(existingUser);
            var entity = existingUser;

            if (response.Data)
            {
                // 1. Actualizar/Asignar contraseña (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var existingCred = _unitOfWork.Usuarios.GetPasswordCredentialAsync(entity.Id, CancellationToken.None).GetAwaiter().GetResult();
                    var (hash, salt) = _hasher.HashPassword(dto.Password);

                    if (existingCred != null)
                    {
                        existingCred.Hash = hash;
                        existingCred.Salt = salt;
                        existingCred.UpdatedAt = DateTime.UtcNow;
                        existingCred.UpdatedBy = "system";
                        _unitOfWork.Credenciales.Update(existingCred);
                    }
                    else
                    {
                        var catCred = _unitOfWork.CatCredenciales.GetAll()
                            .FirstOrDefault(c => c.Descripcion == "PASSWORD");
                        if (catCred != null)
                        {
                            _unitOfWork.Credenciales.Insert(new Credencial
                            {
                                IdUsuario = entity.Id,
                                IdCredencial = catCred.Id,
                                Hash = hash,
                                Salt = salt,
                                IsActive = true,
                                CreatedBy = "system",
                                UpdatedBy = ""
                            });
                        }
                    }
                }

                // 1.2 Actualizar/Asignar PIN (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Pin))
                {
                    var existingCred = _unitOfWork.Usuarios.GetCredentialByTypeAsync(entity.Id, "PIN", CancellationToken.None).GetAwaiter().GetResult();
                    var (hash, salt) = _hasher.HashPassword(dto.Pin);

                    if (existingCred != null)
                    {
                        existingCred.Hash = hash;
                        existingCred.Salt = salt;
                        existingCred.UpdatedAt = DateTime.UtcNow;
                        existingCred.UpdatedBy = "system";
                        _unitOfWork.Credenciales.Update(existingCred);
                    }
                    else
                    {
                        var catCred = _unitOfWork.CatCredenciales.GetAll()
                            .FirstOrDefault(c => c.Descripcion == "PIN");
                        if (catCred != null)
                        {
                            _unitOfWork.Credenciales.Insert(new Credencial
                            {
                                IdUsuario = entity.Id,
                                IdCredencial = catCred.Id,
                                Hash = hash,
                                Salt = salt,
                                IsActive = true,
                                CreatedBy = "system",
                                UpdatedBy = ""
                            });
                        }
                    }
                }

                // 2. Actualizar rol (si se proporciona)
                if (dto.IdRol.HasValue)
                {
                    var userRoles = _unitOfWork.UsuarioRoles.GetAll()
                        .Where(ur => ur.UsuarioId == entity.Id)
                        .ToList();

                    var hasThisRole = userRoles.Any(ur => ur.IdRol == dto.IdRol.Value);
                    if (!hasThisRole)
                    {
                        // Remover roles anteriores
                        foreach (var ur in userRoles)
                        {
                            _unitOfWork.UsuarioRoles.Delete(ur.Id);
                        }

                        // Agregar nuevo rol
                        _unitOfWork.UsuarioRoles.Insert(new UsuarioRol
                        {
                            UsuarioId = entity.Id,
                            IdRol = dto.IdRol.Value,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                response.isSuccess = true;
                response.Message = "Usuario modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            // Protección de turno: no eliminar usuario con turno de caja abierto
            var hasOpenTurno = _unitOfWork.Usuarios.HasOpenTurnoAsync(id, CancellationToken.None).GetAwaiter().GetResult();
            if (hasOpenTurno)
            {
                response.isSuccess = false;
                response.Message = "No es posible eliminar al usuario porque tiene un turno de caja abierto en el POS. Cierre el turno antes de proceder.";
                return response;
            }

            response.Data = _unitOfWork.Usuarios.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario eliminado correctamente";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "No se pudo eliminar, el usuario no existe";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<UsuarioDTO> Get(int id)
    {
        var response = new Response<UsuarioDTO>();
        try
        {
            var entity = _unitOfWork.Usuarios.Get(id);
            
            if (entity != null)
            {
                response.Data = MapToUsuarioDto(entity);
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<UsuarioDTO>> GetAll()
    {
        var response = new Response<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = _unitOfWork.Usuarios.GetAll();
            response.Data = list.Select(MapToUsuarioDto).ToList();
            response.isSuccess = true;
            response.Message = "Usuarios obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<UsuarioDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioDTO>>();
        try
        {
            var count = _unitOfWork.Usuarios.Count();
            var list = _unitOfWork.Usuarios.GetAllWithPagination(page, pageSize);
            response.Data = list.Select(MapToUsuarioDto).ToList();
            response.PageNumber = page;
            response.TotalCount = count;
            response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            response.isSuccess = true;
            response.Message = "Usuarios paginados obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.Data = _unitOfWork.Usuarios.Count();
            response.isSuccess = true;
            response.Message = "Conteo obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var validation = await _validationRules.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var entity = _mapper.Map<Usuario>(dto);

            // Spec 024: Candado de Supervisor
            if (!string.IsNullOrWhiteSpace(dto.PinSupervisor))
            {
                var (supHash, supSalt) = _hasher.HashPassword(dto.PinSupervisor);
                entity.PinSupervisorHash = supHash;
                entity.PinSupervisorSalt = supSalt;
                entity.PinIntentosFallidos = 0;
                entity.PinBloqueadoHasta = null;
            }

            response.Data = await _unitOfWork.Usuarios.InsertAsync(entity);

            if (response.Data)
            {
                // 1. Asignar contraseña (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var catCreds = await _unitOfWork.CatCredenciales.GetAllAsync();
                    var catCred = catCreds.FirstOrDefault(c => c.Descripcion == "PASSWORD");
                    if (catCred != null)
                    {
                        var (hash, salt) = _hasher.HashPassword(dto.Password);
                        await _unitOfWork.Credenciales.InsertAsync(new Credencial
                        {
                            IdUsuario = entity.Id,
                            IdCredencial = catCred.Id,
                            Hash = hash,
                            Salt = salt,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                // 1.2 Asignar PIN de acceso rápido (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Pin))
                {
                    var catCreds = await _unitOfWork.CatCredenciales.GetAllAsync();
                    var catCred = catCreds.FirstOrDefault(c => c.Descripcion == "PIN");
                    if (catCred != null)
                    {
                        var (hash, salt) = _hasher.HashPassword(dto.Pin);
                        await _unitOfWork.Credenciales.InsertAsync(new Credencial
                        {
                            IdUsuario = entity.Id,
                            IdCredencial = catCred.Id,
                            Hash = hash,
                            Salt = salt,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                // 2. Asignar rol (si se proporciona)
                if (dto.IdRol.HasValue)
                {
                    await _unitOfWork.UsuarioRoles.InsertAsync(new UsuarioRol
                    {
                        UsuarioId = entity.Id,
                        IdRol = dto.IdRol.Value,
                        IsActive = true,
                        CreatedBy = "system",
                        UpdatedBy = ""
                    });
                }

                response.isSuccess = true;
                response.Message = "Usuario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var validation = await _validationRules.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var existingUser = await _unitOfWork.Usuarios.GetAsync(dto.Id);
            if (existingUser == null)
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
                return response;
            }

            // Protección de turno: no desactivar usuario con turno de caja abierto
            if (!dto.IsActive && existingUser.IsActive)
            {
                var hasOpenTurno = await _unitOfWork.Usuarios.HasOpenTurnoAsync(dto.Id, CancellationToken.None);
                if (hasOpenTurno)
                {
                    response.isSuccess = false;
                    response.Message = "No es posible desactivar al usuario porque tiene un turno de caja abierto en el POS. Cierre el turno antes de continuar.";
                    return response;
                }
            }

            existingUser.NombreCompleto = dto.NombreCompleto;
            existingUser.Correo = dto.Correo;
            existingUser.IdEmpresa = dto.IdEmpresa;
            existingUser.IdSucursal = dto.IdSucursal;
            existingUser.IsActive = dto.IsActive;

            // Spec 024: Candado de Supervisor
            if (!string.IsNullOrWhiteSpace(dto.PinSupervisor))
            {
                var (supHash, supSalt) = _hasher.HashPassword(dto.PinSupervisor);
                existingUser.PinSupervisorHash = supHash;
                existingUser.PinSupervisorSalt = supSalt;
                existingUser.PinIntentosFallidos = 0;
                existingUser.PinBloqueadoHasta = null;
            }
            else if (dto.DesbloquearPinSupervisor == true)
            {
                existingUser.PinIntentosFallidos = 0;
                existingUser.PinBloqueadoHasta = null;
            }

            response.Data = await _unitOfWork.Usuarios.UpdateAsync(existingUser);
            var entity = existingUser;

            if (response.Data)
            {
                // 1. Actualizar/Asignar contraseña (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var existingCred = await _unitOfWork.Usuarios.GetPasswordCredentialAsync(entity.Id, CancellationToken.None);
                    var (hash, salt) = _hasher.HashPassword(dto.Password);

                    if (existingCred != null)
                    {
                        existingCred.Hash = hash;
                        existingCred.Salt = salt;
                        existingCred.UpdatedAt = DateTime.UtcNow;
                        existingCred.UpdatedBy = "system";
                        await _unitOfWork.Credenciales.UpdateAsync(existingCred);
                    }
                    else
                    {
                        var catCreds = await _unitOfWork.CatCredenciales.GetAllAsync();
                        var catCred = catCreds.FirstOrDefault(c => c.Descripcion == "PASSWORD");
                        if (catCred != null)
                        {
                            await _unitOfWork.Credenciales.InsertAsync(new Credencial
                            {
                                IdUsuario = entity.Id,
                                IdCredencial = catCred.Id,
                                Hash = hash,
                                Salt = salt,
                                IsActive = true,
                                CreatedBy = "system",
                                UpdatedBy = ""
                            });
                        }
                    }
                }

                // 1.2 Actualizar/Asignar PIN (si se proporciona)
                if (!string.IsNullOrWhiteSpace(dto.Pin))
                {
                    var existingCred = await _unitOfWork.Usuarios.GetCredentialByTypeAsync(entity.Id, "PIN", CancellationToken.None);
                    var (hash, salt) = _hasher.HashPassword(dto.Pin);

                    if (existingCred != null)
                    {
                        existingCred.Hash = hash;
                        existingCred.Salt = salt;
                        existingCred.UpdatedAt = DateTime.UtcNow;
                        existingCred.UpdatedBy = "system";
                        await _unitOfWork.Credenciales.UpdateAsync(existingCred);
                    }
                    else
                    {
                        var catCreds = await _unitOfWork.CatCredenciales.GetAllAsync();
                        var catCred = catCreds.FirstOrDefault(c => c.Descripcion == "PIN");
                        if (catCred != null)
                        {
                            await _unitOfWork.Credenciales.InsertAsync(new Credencial
                            {
                                IdUsuario = entity.Id,
                                IdCredencial = catCred.Id,
                                Hash = hash,
                                Salt = salt,
                                IsActive = true,
                                CreatedBy = "system",
                                UpdatedBy = ""
                            });
                        }
                    }
                }

                // 2. Actualizar rol (si se proporciona)
                if (dto.IdRol.HasValue)
                {
                    var userRolesList = await _unitOfWork.UsuarioRoles.GetAllAsync();
                    var userRoles = userRolesList.Where(ur => ur.UsuarioId == entity.Id).ToList();

                    var hasThisRole = userRoles.Any(ur => ur.IdRol == dto.IdRol.Value);
                    if (!hasThisRole)
                    {
                        // Remover roles anteriores
                        foreach (var ur in userRoles)
                        {
                            await _unitOfWork.UsuarioRoles.DeleteAsync(ur.Id);
                        }

                        // Agregar nuevo rol
                        await _unitOfWork.UsuarioRoles.InsertAsync(new UsuarioRol
                        {
                            UsuarioId = entity.Id,
                            IdRol = dto.IdRol.Value,
                            IsActive = true,
                            CreatedBy = "system",
                            UpdatedBy = ""
                        });
                    }
                }

                response.isSuccess = true;
                response.Message = "Usuario modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            // Protección de turno: no eliminar usuario con turno de caja abierto
            var hasOpenTurno = await _unitOfWork.Usuarios.HasOpenTurnoAsync(id, CancellationToken.None);
            if (hasOpenTurno)
            {
                response.isSuccess = false;
                response.Message = "No es posible eliminar al usuario porque tiene un turno de caja abierto en el POS. Cierre el turno antes de proceder.";
                return response;
            }

            response.Data = await _unitOfWork.Usuarios.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario eliminado correctamente";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "No se pudo eliminar, el usuario no existe";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<UsuarioDTO>> GetAsync(int id)
    {
        var response = new Response<UsuarioDTO>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetAsync(id);
            
            if (entity != null)
            {
                response.Data = MapToUsuarioDto(entity);
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<UsuarioDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = await _unitOfWork.Usuarios.GetAllAsync();
            response.Data = list.Select(MapToUsuarioDto).ToList();
            response.isSuccess = true;
            response.Message = "Usuarios obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<UsuarioDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioDTO>>();
        try
        {
            var count = await _unitOfWork.Usuarios.CountAsync();
            var list = await _unitOfWork.Usuarios.GetAllWithPaginationAsync(page, pageSize);
            response.Data = list.Select(MapToUsuarioDto).ToList();
            response.PageNumber = page;
            response.TotalCount = count;
            response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            response.isSuccess = true;
            response.Message = "Usuarios paginados obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.Data = await _unitOfWork.Usuarios.CountAsync();
            response.isSuccess = true;
            response.Message = "Conteo obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion
    
    #region Metodos de Seguridad y Operativos

    public async Task<Response<UsuarioDTO?>> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct)
    {
        var response = new Response<UsuarioDTO?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetByCorreoWithRolesAndCredentialsAsync(correo, ct);
            response.Data = _mapper.Map<UsuarioDTO?>(entity);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<UsuarioDTO?>> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct)
    {
        var response = new Response<UsuarioDTO?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetByUserOrEmailWithAuthGraphAsync(userOrEmail, ct);
            response.Data = _mapper.Map<UsuarioDTO?>(entity);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IReadOnlyList<string>>> GetRoleNamesAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<IReadOnlyList<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetRoleNamesAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IReadOnlyList<string>>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<IReadOnlyList<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetAccesoPathsByUsuarioIdAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<object?>> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<object?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetPasswordCredentialAsync(usuarioId, ct);
            response.Data = _mapper.Map<object?>(entity); // Mapear a CredencialDTO si existe
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.HasOpenTurnoAsync(idUsuario, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<List<string>>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<List<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetPermissionKeysByUsuarioIdAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<string?>> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<string?>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetPermissionsVersionAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion
}