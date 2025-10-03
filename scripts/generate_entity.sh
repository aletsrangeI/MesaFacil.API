#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: generate_entity.sh <Name> [--plural Plural] [--dry-run]

Options:
  --plural    Provide an explicit PascalCase plural form.
  --dry-run   Show the operations without writing to disk.
  -h, --help  Show this help message.
USAGE
}

error() {
  echo "Error: $1" >&2
  exit 1
}

ensure_pascal() {
  local value="$1"
  if [[ ! $value =~ ^[A-Z][A-Za-z0-9]*$ ]]; then
    error "'$value' must be in PascalCase"
  fi
}

default_plural() {
  local value="$1"
  if [[ $value =~ [^aeiou]y$ ]]; then
    echo "${value::-1}ies"
  elif [[ $value =~ s$ ]]; then
    echo "${value}es"
  else
    echo "${value}s"
  fi
}

to_kebab() {
  local value="$1"
  echo "$value" | sed -E 's/([a-z0-9])([A-Z])/\1-\2/g' | tr '[:upper:]' '[:lower:]'
}

insert_after_last_using() {
  local file="$1"
  local line="$2"
  local tmp="$3"
  awk -v newline="$line" '
    BEGIN { inserted=0 }
    /^using / { last=NR }
    { lines[NR]=$0 }
    END {
      for (i=1; i<=NR; i++) {
        print lines[i]
        if (!inserted && i==last) {
          print newline
          inserted=1
        }
      }
      if (!inserted) {
        print newline
      }
    }
  ' "$file" > "$tmp"
}

ensure_directory() {
  local dir="$1"
  if [[ ! -d $dir ]]; then
    mkdir -p "$dir"
  fi
}

DRY_RUN=0
NAME=""
PLURAL=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    -h|--help)
      usage
      exit 0
      ;;
    --plural)
      shift || error "Missing value for --plural"
      PLURAL="$1"
      ;;
    --dry-run)
      DRY_RUN=1
      ;;
    --*)
      error "Unknown option: $1"
      ;;
    *)
      if [[ -z $NAME ]]; then
        NAME="$1"
      else
        error "Unexpected argument: $1"
      fi
      ;;
  esac
  shift
done

[[ -n $NAME ]] || { usage; error "Entity name is required"; }
ensure_pascal "$NAME"

if [[ -n $PLURAL ]]; then
  ensure_pascal "$PLURAL"
else
  PLURAL=$(default_plural "$NAME")
  ensure_pascal "$PLURAL"
fi

CAMEL="${NAME,}"
CAMEL_PLURAL="${PLURAL,}"
KEBAB=$(to_kebab "$NAME")

ROOT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)

create_file() {
  local path="$1"
  local content="$2"
  if [[ -e $path ]]; then
    error "File already exists: $path"
  fi
  if [[ $DRY_RUN -eq 1 ]]; then
    echo "[dry-run] create $path"
    return
  fi
  ensure_directory "$(dirname "$path")"
  printf '%s\n' "$content" > "$path"
  echo "created $path"
}

render_template() {
  local template="$1"
  local result="${template//__NAME__/$NAME}"
  result="${result//__PLURAL__/$PLURAL}"
  result="${result//__KEBAB__/$KEBAB}"
  result="${result//__CAMEL__/$CAMEL}"
  printf '%s' "$result"
}

update_file_with_temp() {
  local path="$1"
  local callback="$2"
  local tmp
  tmp=$(mktemp)
  "$callback" "$path" "$tmp"
  if ! cmp -s "$path" "$tmp"; then
    if [[ $DRY_RUN -eq 1 ]]; then
      echo "[dry-run] update $path"
      rm -f "$tmp"
      return
    fi
    mv "$tmp" "$path"
    echo "updated $path"
  else
    rm -f "$tmp"
  fi
}

add_dbset_impl() {
  local file="$1"
  local tmp="$2"
  awk -v newline="    public DbSet<$NAME> $PLURAL { get; set; }" '
    BEGIN { inserted=0 }
    {
      if (!inserted && $0 ~ /^\s*protected override void OnModelCreating/) {
        print newline
        inserted=1
      }
      print
    }
  ' "$file" > "$tmp"
}

add_dbset() {
  local file="$ROOT_DIR/Persistence/Context/ApplicationDbContext.cs"
  local line="    public DbSet<$NAME> $PLURAL { get; set; }"
  if grep -qF "$line" "$file"; then
    return
  fi
  update_file_with_temp "$file" add_dbset_impl
}

update_mappings_profile() {
  local file="$ROOT_DIR/UseCases/Common/Mapping/MappingsProfile.cs"
  local using_line="using DTO.$NAME;"
  local map_line="        CreateMap<$NAME, ${NAME}DTO>().ReverseMap();"
  local tmp
  tmp=$(mktemp)
  cp "$file" "$tmp"
  local changed=0

  if ! grep -qF "$using_line" "$tmp"; then
    local tmp2
    tmp2=$(mktemp)
    insert_after_last_using "$tmp" "$using_line" "$tmp2"
    mv "$tmp2" "$tmp"
    changed=1
  fi

  if ! grep -qF "$map_line" "$tmp"; then
    sed -i "/CreateMap<Catalog, CatalogDTO>().ReverseMap();/a\\        CreateMap<$NAME, ${NAME}DTO>().ReverseMap();" "$tmp"
    changed=1
  fi

  if [[ $changed -eq 0 ]]; then
    rm -f "$tmp"
    return
  fi

  if [[ $DRY_RUN -eq 1 ]]; then
    echo "[dry-run] update $file"
    rm -f "$tmp"
  else
    mv "$tmp" "$file"
    echo "updated $file"
  fi
}

update_usecases_config() {
  local file="$ROOT_DIR/UseCases/ConfigureServices.cs"
  local using_line="using UseCases.$PLURAL;"
  local service_line="        services.AddScoped<I${NAME}Application, ${NAME}Application>();"
  local validator_line="        services.AddTransient<${NAME}DTOValidator>();"
  local tmp
  tmp=$(mktemp)
  cp "$file" "$tmp"
  local changed=0

  if ! grep -qF "$using_line" "$tmp"; then
    local tmp2
    tmp2=$(mktemp)
    insert_after_last_using "$tmp" "$using_line" "$tmp2"
    mv "$tmp2" "$tmp"
    changed=1
  fi

  if ! grep -qF "$service_line" "$tmp"; then
    sed -i "/services.AddScoped<ICatalogApplication, CatalogApplication>();/a\\        services.AddScoped<I${NAME}Application, ${NAME}Application>();" "$tmp"
    changed=1
  fi

  if ! grep -qF "$validator_line" "$tmp"; then
    sed -i "/services.AddTransient<CatalogDTOValidator>();/a\\        services.AddTransient<${NAME}DTOValidator>();" "$tmp"
    changed=1
  fi

  if [[ $changed -eq 0 ]]; then
    rm -f "$tmp"
    return
  fi

  if [[ $DRY_RUN -eq 1 ]]; then
    echo "[dry-run] update $file"
    rm -f "$tmp"
  else
    mv "$tmp" "$file"
    echo "updated $file"
  fi
}

update_persistence_config() {
  local file="$ROOT_DIR/Persistence/ConfigureServices.cs"
  local registration="        services.AddScoped<I${NAME}Repository, ${NAME}Repository>();"
  if grep -qF "$registration" "$file"; then
    return
  fi
  local tmp
  tmp=$(mktemp)
  cp "$file" "$tmp"
  sed -i "/services.AddScoped<IUnitOfWork, UnitOfWork>();/i\\        services.AddScoped<I${NAME}Repository, ${NAME}Repository>();" "$tmp"
  if [[ $DRY_RUN -eq 1 ]]; then
    echo "[dry-run] update $file"
    rm -f "$tmp"
  else
    mv "$tmp" "$file"
    echo "updated $file"
  fi
}

update_iunitofwork_impl() {
  local file="$1"
  local tmp="$2"
  awk -v newline="    I${NAME}Repository $PLURAL { get; }" '
    /^}/ {
      print newline
    }
    { print }
  ' "$file" > "$tmp"
}

update_iunitofwork() {
  local file="$ROOT_DIR/Interface/Persistence/IUnitOfWork.cs"
  local property="    I${NAME}Repository $PLURAL { get; }"
  if grep -qF "$property" "$file"; then
    return
  fi
  update_file_with_temp "$file" update_iunitofwork_impl
}

update_unitofwork() {
  local file="$ROOT_DIR/Persistence/Repositories/UnitOfWork.cs"
  local property="    public I${NAME}Repository $PLURAL { get; }"
  local parameter="I${NAME}Repository ${CAMEL}Repository"
  local assignment="        $PLURAL = ${CAMEL}Repository;"

  local tmp
  tmp=$(mktemp)
  cp "$file" "$tmp"
  local changed=0

  if ! grep -qF "$property" "$tmp"; then
    sed -i "/public ICatalogRepository Catalogs { get; }/a\\    public I${NAME}Repository $PLURAL { get; }" "$tmp"
    changed=1
  fi

  if ! grep -q "$parameter" "$tmp"; then
    sed -i "s/ICatalogRepository catalogRepository)/ICatalogRepository catalogRepository,\\\n        I${NAME}Repository ${CAMEL}Repository)/" "$tmp"
    changed=1
  fi

  if ! grep -qF "$assignment" "$tmp"; then
    sed -i "/Catalogs = catalogRepository;/a\\        $PLURAL = ${CAMEL}Repository;" "$tmp"
    changed=1
  fi

  if [[ $changed -eq 0 ]]; then
    rm -f "$tmp"
    return
  fi

  if [[ $DRY_RUN -eq 1 ]]; then
    echo "[dry-run] update $file"
    rm -f "$tmp"
  else
    mv "$tmp" "$file"
    echo "updated $file"
  fi
}

create_files() {
  local template

  template=$(cat <<'TEMPLATE'
namespace Domain.Entities;

public class __NAME__ : BaseAuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
TEMPLATE
)
  create_file "$ROOT_DIR/Domain/Entities/$NAME.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
namespace DTO.__NAME__;

public class __NAME__DTO
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
TEMPLATE
)
  create_file "$ROOT_DIR/DTO/$NAME/${NAME}DTO.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/Validator/${NAME}DTOValidator.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
using Domain.Entities;

namespace Interface.Persistence;

public interface I__NAME__Repository : IGenericRepository<__NAME__> {}
TEMPLATE
)
  create_file "$ROOT_DIR/Interface/Persistence/I${NAME}Repository.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/Interface/UseCases/I${NAME}Application.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/UseCases/$PLURAL/${NAME}Application.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/Persistence/Repositories/${NAME}Repository.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/Persistence/Configurations/${NAME}Configuration.cs" "$(render_template "$template")"

  template=$(cat <<'TEMPLATE'
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
TEMPLATE
)
  create_file "$ROOT_DIR/WebApi/Modules/Endpoints/$NAME.cs" "$(render_template "$template")"
}

main() {
  create_files
  add_dbset
  update_mappings_profile
  update_usecases_config
  update_persistence_config
  update_iunitofwork
  update_unitofwork
}

main
