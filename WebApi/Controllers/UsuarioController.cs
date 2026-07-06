using Common;
using DTO.Usuario;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsuarioController : Controller
{
    private readonly IUsuarioApplication _usuarioApplication;

    public UsuarioController(IUsuarioApplication usuarioApplication)
    {
        _usuarioApplication = usuarioApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] UsuarioDTO usuario)
    {
        var response = _usuarioApplication.Insert(usuario);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    [AllowAnonymous]
    public ActionResult<Response<IEnumerable<UsuarioDTO>>> GetAll()
    {
        var response = _usuarioApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<UsuarioDTO>> GetById(int id)
    {
        var response = _usuarioApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] UsuarioDTO usuario)
    {
        var response = _usuarioApplication.Update(usuario);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _usuarioApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<UsuarioDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _usuarioApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _usuarioApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] UsuarioDTO usuario)
    {
        var response = await _usuarioApplication.InsertAsync(usuario);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<UsuarioDTO>>>> GetAllAsync()
    {
        var response = await _usuarioApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<UsuarioDTO>>> GetByIdAsync(int id)
    {
        var response = await _usuarioApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] UsuarioDTO usuario)
    {
        var response = await _usuarioApplication.UpdateAsync(usuario);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _usuarioApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<UsuarioDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _usuarioApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _usuarioApplication.CountAsync();
        return Ok(response);
    }

    #endregion
    
    #region Metodos de Seguridad y Operativos

    [HttpGet("GetByCorreoWithRolesAndCredentialsAsync/{correo}")]
    public async Task<ActionResult<Response<UsuarioDTO?>>> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetByCorreoWithRolesAndCredentialsAsync(correo, ct);
        return Ok(response);
    }

    [HttpGet("GetByUserOrEmailWithAuthGraphAsync/{userOrEmail}")]
    public async Task<ActionResult<Response<UsuarioDTO?>>> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetByUserOrEmailWithAuthGraphAsync(userOrEmail, ct);
        return Ok(response);
    }

    [HttpGet("GetRoleNamesAsync/{usuarioId}")]
    public async Task<ActionResult<Response<IReadOnlyList<string>>>> GetRoleNamesAsync(int usuarioId, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetRoleNamesAsync(usuarioId, ct);
        return Ok(response);
    }

    [HttpGet("GetAccesoPathsByUsuarioIdAsync/{usuarioId}")]
    public async Task<ActionResult<Response<IReadOnlyList<string>>>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetAccesoPathsByUsuarioIdAsync(usuarioId, ct);
        return Ok(response);
    }

    [HttpGet("GetPasswordCredentialAsync/{usuarioId}")]
    public async Task<ActionResult<Response<object?>>> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetPasswordCredentialAsync(usuarioId, ct);
        return Ok(response);
    }

    [HttpGet("HasOpenTurnoAsync/{idUsuario}")]
    public async Task<ActionResult<Response<bool>>> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        var response = await _usuarioApplication.HasOpenTurnoAsync(idUsuario, ct);
        return Ok(response);
    }

    [HttpGet("GetPermissionKeysByUsuarioIdAsync/{usuarioId}")]
    public async Task<ActionResult<Response<List<string>>>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetPermissionKeysByUsuarioIdAsync(usuarioId, ct);
        return Ok(response);
    }

    [HttpGet("GetPermissionsVersionAsync/{usuarioId}")]
    public async Task<ActionResult<Response<string?>>> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct)
    {
        var response = await _usuarioApplication.GetPermissionsVersionAsync(usuarioId, ct);
        return Ok(response);
    }

    #endregion
}