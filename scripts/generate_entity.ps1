[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [string]$Plural,
    [switch]$DryRun
)

function Ensure-Pascal {
    param(
        [string]$Value,
        [string]$Label
    )

    if ($Value -notmatch '^[A-Z][A-Za-z0-9]*$') {
        throw "${Label} must be in PascalCase"
    }

    return $Value
}

function Get-DefaultPlural {
    param([string]$Value)

    if ($Value -match '[^aeiou]y$') {
        return $Value.Substring(0, $Value.Length - 1) + 'ies'
    }

    if ($Value.EndsWith('s')) {
        return $Value + 'es'
    }

    return $Value + 's'
}

function To-Camel {
    param([string]$Value)

    if ($Value.Length -le 1) {
        return $Value.ToLower()
    }

    return $Value.Substring(0,1).ToLower() + $Value.Substring(1)
}

function To-Kebab {
    param([string]$Value)

    return ([regex]::Replace($Value, '([a-z0-9])([A-Z])', '$1-$2')).ToLower()
}

$Name = Ensure-Pascal -Value $Name -Label 'Name'
if ([string]::IsNullOrWhiteSpace($Plural)) {
    $Plural = Get-DefaultPlural -Value $Name
}
$Plural = Ensure-Pascal -Value $Plural -Label 'Plural'

$Camel = To-Camel -Value $Name
$CamelPlural = To-Camel -Value $Plural
$Kebab = To-Kebab -Value $Name

$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).ProviderPath
$NewLine = [Environment]::NewLine

function Ensure-TrailingNewLine {
    param([string]$Text)

    if (-not $Text.EndsWith($NewLine)) {
        return $Text + $NewLine
    }

    return $Text
}

function New-ScaffoldFile {
    param(
        [string]$RelativePath,
        [string]$Content
    )

    $path = Join-Path $Root $RelativePath

    if (Test-Path -LiteralPath $path) {
        throw "File already exists: $RelativePath"
    }

    if ($DryRun) {
        Write-Host "[dry-run] create $RelativePath"
        return
    }

    $directory = Split-Path -Parent $path
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $finalContent = Ensure-TrailingNewLine -Text $Content
    [IO.File]::WriteAllText($path, $finalContent, [System.Text.Encoding]::UTF8)
    Write-Host "created $RelativePath"
}

function Update-File {
    param(
        [string]$RelativePath,
        [ScriptBlock]$Updater
    )

    $path = Join-Path $Root $RelativePath
    $original = Get-Content -LiteralPath $path -Raw
    $updated = & $Updater $original
    if ($updated -eq $null) {
        throw "Updater for $RelativePath returned null"
    }
    $updated = Ensure-TrailingNewLine -Text $updated

    if ($updated -ne $original) {
        if ($DryRun) {
            Write-Host "[dry-run] update $RelativePath"
        } else {
            [IO.File]::WriteAllText($path, $updated, [System.Text.Encoding]::UTF8)
            Write-Host "updated $RelativePath"
        }
    }
}

function Insert-Using {
    param(
        [string]$Text,
        [string]$UsingLine
    )

    if ($Text -like "*$UsingLine*") {
        return $Text
    }

    $matches = [regex]::Matches($Text, '^using .+$', [RegexOptions]::Multiline)
    if ($matches.Count -gt 0) {
        $last = $matches[$matches.Count - 1]
        $insertPos = $last.Index + $last.Length
        return $Text.Insert($insertPos, $NewLine + $UsingLine)
    }

    return $UsingLine + $NewLine + $Text
}

function Add-DbSet {
    $line = "    public DbSet<$Name> $Plural { get; set; }"
    Update-File -RelativePath 'Persistence/Context/ApplicationDbContext.cs' -Updater {
        param($text)
        if ($text -like "*$line*") {
            return $text
        }
        $anchor = 'protected override void OnModelCreating'
        $index = $text.IndexOf($anchor, [StringComparison]::Ordinal)
        if ($index -lt 0) {
            throw 'Unable to locate OnModelCreating in ApplicationDbContext.cs'
        }
        return $text.Insert($index, $line + $NewLine)
    }
}

function Update-MappingsProfile {
    $usingLine = "using DTO.$Name;"
    $mapLine = "        CreateMap<$Name, ${Name}DTO>().ReverseMap();"
    Update-File -RelativePath 'UseCases/Common/Mapping/MappingsProfile.cs' -Updater {
        param($text)
        $result = Insert-Using -Text $text -UsingLine $usingLine
        if ($result -notlike "*$mapLine*") {
            $anchor = 'CreateMap<Catalog, CatalogDTO>().ReverseMap();'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate mapping anchor in MappingsProfile.cs'
            }
            $result = $result.Insert($idx + $anchor.Length, $NewLine + $mapLine)
        }
        return $result
    }
}

function Update-UseCasesConfig {
    $usingLine = "using UseCases.$Plural;"
    $serviceLine = "        services.AddScoped<I${Name}Application, ${Name}Application>();"
    $validatorLine = "        services.AddTransient<${Name}DTOValidator>();"
    Update-File -RelativePath 'UseCases/ConfigureServices.cs' -Updater {
        param($text)
        $result = Insert-Using -Text $text -UsingLine $usingLine
        if ($result -notlike "*$serviceLine*") {
            $anchor = 'services.AddScoped<ICatalogApplication, CatalogApplication>();'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate Catalog application registration in UseCases.ConfigureServices.cs'
            }
            $result = $result.Insert($idx + $anchor.Length, $NewLine + $serviceLine)
        }
        if ($result -notlike "*$validatorLine*") {
            $anchor = 'services.AddTransient<CatalogDTOValidator>();'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate Catalog validator registration in UseCases.ConfigureServices.cs'
            }
            $result = $result.Insert($idx + $anchor.Length, $NewLine + $validatorLine)
        }
        return $result
    }
}

function Update-PersistenceConfig {
    $registration = "        services.AddScoped<I${Name}Repository, ${Name}Repository>();"
    Update-File -RelativePath 'Persistence/ConfigureServices.cs' -Updater {
        param($text)
        if ($text -like "*$registration*") {
            return $text
        }
        $anchor = 'services.AddScoped<IUnitOfWork, UnitOfWork>();'
        $idx = $text.IndexOf($anchor, [StringComparison]::Ordinal)
        if ($idx -lt 0) {
            throw 'Unable to locate UnitOfWork registration in Persistence.ConfigureServices.cs'
        }
        return $text.Insert($idx, $registration + $NewLine)
    }
}

function Update-IUnitOfWork {
    $property = "    I${Name}Repository $Plural { get; }"
    Update-File -RelativePath 'Interface/Persistence/IUnitOfWork.cs' -Updater {
        param($text)
        if ($text -like "*$property*") {
            return $text
        }
        $idx = $text.LastIndexOf('}', [StringComparison]::Ordinal)
        if ($idx -lt 0) {
            throw 'Unable to locate closing brace in IUnitOfWork.cs'
        }
        return $text.Insert($idx, $property + $NewLine)
    }
}

function Update-UnitOfWork {
    $property = "    public I${Name}Repository $Plural { get; }"
    $parameter = "I${Name}Repository ${Camel}Repository"
    $assignment = "        $Plural = ${Camel}Repository;"
    Update-File -RelativePath 'Persistence/Repositories/UnitOfWork.cs' -Updater {
        param($text)
        $result = $text
        if ($result -notlike "*$property*") {
            $anchor = 'public ICatalogRepository Catalogs { get; }'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate Catalog property in UnitOfWork.cs'
            }
            $result = $result.Insert($idx + $anchor.Length, $NewLine + $property)
        }
        if ($result -notlike "*$parameter*") {
            $anchor = 'ICatalogRepository catalogRepository)'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate catalogRepository parameter in UnitOfWork.cs'
            }
            $replacement = "ICatalogRepository catalogRepository,$NewLine        $parameter)"
            $result = $result.Remove($idx, $anchor.Length).Insert($idx, $replacement)
        }
        if ($result -notlike "*$assignment*") {
            $anchor = '        Catalogs = catalogRepository;'
            $idx = $result.IndexOf($anchor, [StringComparison]::Ordinal)
            if ($idx -lt 0) {
                throw 'Unable to locate Catalog assignment in UnitOfWork.cs'
            }
            $result = $result.Insert($idx + $anchor.Length, $NewLine + $assignment)
        }
        return $result
    }
}

function Get-Template {
    param([string]$Template)

    return $Template.Replace('__NAME__', $Name)
                    .Replace('__PLURAL__', $Plural)
                    .Replace('__KEBAB__', $Kebab)
}

function Create-Files {
    New-ScaffoldFile -RelativePath "Domain/Entities/$Name.cs" -Content (Get-Template @"
namespace Domain.Entities;

public class __NAME__ : BaseAuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
"@)

    New-ScaffoldFile -RelativePath "DTO/$Name/${Name}DTO.cs" -Content (Get-Template @"
namespace DTO.__NAME__;

public class __NAME__DTO
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
"@)

    New-ScaffoldFile -RelativePath "Validator/${Name}DTOValidator.cs" -Content (Get-Template @"
using DTO.__NAME__;
using FluentValidation;

namespace Validator;

public class __NAME__DTOValidator : AbstractValidator<__NAME__DTO>
{
    public __NAME__DTOValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("El campo es requerido");
        RuleFor(x => x.Name).NotEmpty().WithMessage("El campo es requerido");
    }
}
"@)

    New-ScaffoldFile -RelativePath "Interface/Persistence/I${Name}Repository.cs" -Content (Get-Template @"
using Domain.Entities;

namespace Interface.Persistence;

public interface I__NAME__Repository : IGenericRepository<__NAME__> {}
"@)

    New-ScaffoldFile -RelativePath "Interface/UseCases/I${Name}Application.cs" -Content (Get-Template @"
using Common;
using DTO.__NAME__;

namespace Interface.UseCases;

public interface I__NAME__Application
{
    #region Metodos sincronos

    Response<bool> Insert(__NAME__DTO dto);
    Response<bool> Update(__NAME__DTO dto);
    Response<bool> Delete(int id);
    Response<__NAME__DTO> Get(int id);
    Response<IEnumerable<__NAME__DTO>> GetAll();
    ResponsePagination<IEnumerable<__NAME__DTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(__NAME__DTO dto);
    Task<Response<bool>> UpdateAsync(__NAME__DTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<__NAME__DTO>> GetAsync(int id);
    Task<Response<IEnumerable<__NAME__DTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<__NAME__DTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
"@)

    New-ScaffoldFile -RelativePath "UseCases/$Plural/${Name}Application.cs" -Content (Get-Template @"
using AutoMapper;
using Common;
using Domain.Entities;
using DTO.__NAME__;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.__PLURAL__;

public class __NAME__Application : I__NAME__Application
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly __NAME__DTOValidator _validationRules;
    private readonly IAppLogger<__NAME__Application> _logger;

    public __NAME__Application(IUnitOfWork unitOfWork, IMapper mapper, __NAME__DTOValidator validationRules,
        IAppLogger<__NAME__Application> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos Sincronos

    public Response<bool> Insert(__NAME__DTO dto)
    {
        var response = new Response<bool>();

        try
        {
            var entity = _mapper.Map<__NAME__>(dto);
            response.Data = _unitOfWork.__PLURAL__.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(__NAME__DTO dto)
    {
        var response = new Response<bool>();

        try
        {
            var entity = _mapper.Map<__NAME__>(dto);
            response.Data = _unitOfWork.__PLURAL__.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro actualizado correctamente";
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
            response.Data = _unitOfWork.__PLURAL__.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<__NAME__DTO> Get(int id)
    {
        var response = new Response<__NAME__DTO>();

        try
        {
            var entity = _unitOfWork.__PLURAL__.Get(id);
            response.Data = _mapper.Map<__NAME__DTO>(entity);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registro encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<__NAME__DTO>> GetAll()
    {
        var response = new Response<IEnumerable<__NAME__DTO>>();

        try
        {
            var entities = _unitOfWork.__PLURAL__.GetAll();
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(entities);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registros encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<__NAME__DTO>> GetAllWithPagination(int pageNumber, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<__NAME__DTO>>();

        try
        {
            var entities = _unitOfWork.__PLURAL__.GetAllWithPagination(pageNumber, pageSize);
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(entities);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registros encontrados";
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
            response.Data = _unitOfWork.__PLURAL__.Count();

            if (response.Data > 0)
            {
                response.isSuccess = true;
                response.Message = "Total de registros";
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

    public async Task<Response<bool>> InsertAsync(__NAME__DTO dto)
    {
        var response = new Response<bool>();

        try
        {
            var entity = _mapper.Map<__NAME__>(dto);
            response.Data = await _unitOfWork.__PLURAL__.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(__NAME__DTO dto)
    {
        var response = new Response<bool>();

        try
        {
            var entity = _mapper.Map<__NAME__>(dto);
            response.Data = await _unitOfWork.__PLURAL__.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro actualizado correctamente";
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
            response.Data = await _unitOfWork.__PLURAL__.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Registro eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<__NAME__DTO>> GetAsync(int id)
    {
        var response = new Response<__NAME__DTO>();

        try
        {
            var entity = await _unitOfWork.__PLURAL__.GetAsync(id);
            response.Data = _mapper.Map<__NAME__DTO>(entity);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registro encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<__NAME__DTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<__NAME__DTO>>();

        try
        {
            var entities = await _unitOfWork.__PLURAL__.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(entities);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registros encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<__NAME__DTO>>> GetAllWithPaginationAsync(int pageNumber, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<__NAME__DTO>>();

        try
        {
            var entities = await _unitOfWork.__PLURAL__.GetAllWithPaginationAsync(pageNumber, pageSize);
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(entities);

            if (response.Data is not null)
            {
                response.isSuccess = true;
                response.Message = "Registros encontrados";
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
            response.Data = await _unitOfWork.__PLURAL__.CountAsync();

            if (response.Data > 0)
            {
                response.isSuccess = true;
                response.Message = "Total de registros";
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
}
"@)

    New-ScaffoldFile -RelativePath "Persistence/Repositories/${Name}Repository.cs" -Content (Get-Template @"
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class __NAME__Repository : I__NAME__Repository
{
    private readonly ApplicationDbContext _context;

    public __NAME__Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(__NAME__ entity)
    {
        _context.__PLURAL__.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(__NAME__ entity)
    {
        _context.__PLURAL__.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity is null) return false;
        _context.__PLURAL__.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public __NAME__? Get(int id)
    {
        return _context.__PLURAL__.Find(id);
    }

    public IEnumerable<__NAME__> GetAll()
    {
        return _context.__PLURAL__;
    }

    public IEnumerable<__NAME__> GetAllWithPagination(int page, int pageSize)
    {
        return _context.__PLURAL__
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.__PLURAL__.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(__NAME__ entity)
    {
        await _context.__PLURAL__.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(__NAME__ entity)
    {
        _context.__PLURAL__.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity is null) return false;
        _context.__PLURAL__.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<__NAME__?> GetAsync(int id)
    {
        return await _context.__PLURAL__.FindAsync(id);
    }

    public async Task<IEnumerable<__NAME__>> GetAllAsync()
    {
        return await _context.__PLURAL__.ToListAsync();
    }

    public async Task<IEnumerable<__NAME__>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.__PLURAL__
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.__PLURAL__.CountAsync();
    }

    #endregion
}
"@)

    New-ScaffoldFile -RelativePath "Persistence/Configurations/${Name}Configuration.cs" -Content (Get-Template @"
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class __NAME__Configuration : IEntityTypeConfiguration<__NAME__>
{
    public void Configure(EntityTypeBuilder<__NAME__> builder)
    {
        builder.ToTable("__PLURAL__");
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Code).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.CreatedBy).HasMaxLength(128);
        builder.Property(x => x.UpdatedBy).HasMaxLength(128);
    }
}
"@)

    New-ScaffoldFile -RelativePath "WebApi/Modules/Endpoints/$Name.cs" -Content (Get-Template @"
using Common;
using DTO.__NAME__;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class __NAME__Endpoints
{
    public static IEndpointRouteBuilder Map__NAME__Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/__KEBAB__")
            .RequireAuthorization()
            .WithTags("__NAME__")
            .WithOpenApi();

        group.MapPost("/", async (__NAME__DTO dto, I__NAME__Application svc, CancellationToken ct) =>
            {
                var result = await svc.InsertAsync(dto);
                return Results.Ok(result);
            })
            .WithName("__NAME___Create");

        group.MapPut("/{id:int}", async (int id, __NAME__DTO dto, I__NAME__Application svc, CancellationToken ct) =>
            {
                dto.Id = id;
                var result = await svc.UpdateAsync(dto);
                return Results.Ok(result);
            })
            .WithName("__NAME___Update");

        group.MapDelete("/{id:int}", async (int id, I__NAME__Application svc, CancellationToken ct) =>
            {
                var result = await svc.DeleteAsync(id);
                return Results.Ok(result);
            })
            .WithName("__NAME___Delete");

        group.MapGet("/", async (I__NAME__Application svc, CancellationToken ct) =>
            {
                var result = await svc.GetAllAsync();
                return Results.Ok(result);
            })
            .WithName("__NAME___GetAll");

        group.MapGet("/{id:int}",
                async Task<Results<Ok<Response<__NAME__DTO>>, NotFound>> (int id, I__NAME__Application svc) =>
                {
                    var result = await svc.GetAsync(id);
                    return (result is null || result.Data is null)
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(result);
                })
            .WithName("__NAME___GetById");

        group.MapGet("/paged", async (int page, int pageSize, I__NAME__Application svc, CancellationToken ct) =>
            {
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 10 : pageSize;

                var result = await svc.GetAllWithPaginationAsync(page, pageSize);
                return Results.Ok(result);
            })
            .WithName("__NAME___GetPaged");

        group.MapGet("/count", async (I__NAME__Application svc, CancellationToken ct) =>
            {
                var result = await svc.CountAsync();
                return Results.Ok(result);
            })
            .WithName("__NAME___Count");

        return app;
    }
}
"@)
}

Create-Files
Add-DbSet
Update-MappingsProfile
Update-UseCasesConfig
Update-PersistenceConfig
Update-IUnitOfWork
Update-UnitOfWork
