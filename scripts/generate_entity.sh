#!/usr/bin/env bash
set -euo pipefail

# ===============================================
# generate_entity.sh — scaffolding desde Domain
# Uso: ./scripts/generate_entity.sh Menu --plural Menus
# Requisitos:
#   - Debe existir Domain/Entities/<Name>.cs
#   - Crea/actualiza:
#       DTO/<Name>/<Name>DTO.cs
#       Interface/UseCases/I<Name>Application.cs
#       UseCases/<Plural>/<Name>Application.cs
#       Persistence/Repositories/I<Name>Repository.cs
#       Persistence/Repositories/<Name>Repository.cs
#   - Inyecta:
#       MappingsProfile.cs (CreateMap<Domain.Entities.Name, DTO.Name.NameDTO>().ReverseMap())
#       ApplicationDbContext (DbSet<Name> <Plural> { get; set; })
#       UnitOfWork (propiedad y constructor para repositorio)
# ===============================================

# --- util consola ---
info()  { printf "[INFO] %s\n" "$*"; }
warn()  { printf "[WARN] %s\n" "$*" >&2; }
error() { printf "[ERROR] %s\n" "$*" >&2; exit 1; }

# --- helpers de archivo ---
ensure_dir() { mkdir -p "$1"; }
create_file() {
  local path="$1"; shift
  if [[ -f "$path" ]]; then
    warn "Ya existe $path (no se sobreescribe)."
  else
    printf "%s" "$*" > "$path"
    info "Creado: $path"
  fi
}

# --- args ---
if [[ $# -lt 1 ]]; then
  error "Uso: $0 <EntityName> [--plural PluralName]"
fi

NAME="$1"; shift || true
PLURAL=""
while [[ $# -gt 0 ]]; do
  case "$1" in
    --plural)
      PLURAL="${2:-}"; shift 2 || true;;
    *)
      warn "Argumento no reconocido: $1"; shift;;
  esac
done
if [[ -z "${PLURAL:-}" ]]; then
  PLURAL="${NAME}s"
fi

# --- paths ---
SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# === EXTRAER PROPIEDADES DESDE DOMAIN ===
domain_entity_path() {
  echo "$ROOT_DIR/Domain/Entities/${NAME}.cs"
}

# Devuelve las propiedades como líneas "    public <Tipo> <Nombre> { get; set; }"
# Heurística:
# - Ignora atributos [ ... ]
# - Toma propiedades con get/set (misma línea)
# - Limpia inicializadores (= ...;) y 'virtual'
extract_domain_properties() {
  local file
  file="$(domain_entity_path)"
  if [[ ! -f "$file" ]] ; then
    error "No existe el archivo de entidad en: $file"
  fi

  awk '
    /^\s*\[/ { next }
    /^\s*using\b/ { next }
    {
      line=$0
      sub(/\/\/.*/, "", line)
      if (line ~ /^\s*public[ \t]+[A-Za-z0-9_<>,\?\[\]\.]+[ \t]+[A-Za-z0-9_]+[ \t]*\{[^}]*\}/ && line ~ /get;.*set;|set;.*get;/) {
        gsub(/\bvirtual\b[ \t]*/, "", line)
        sub(/=[^;]*;/, ";", line)
        match(line, /^\s*public[ \t]+([A-Za-z0-9_<>,\?\[\]\.]+)[ \t]+([A-Za-z0-9_]+)/, m)
        if (m[1] != "" && m[2] != "") {
          printf("    public %s %s { get; set; }\n", m[1], m[2])
        }
      }
    }
  ' "$file"
}

fallback_dto_properties() {
  cat <<'EOF'
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
EOF
}

# === GENERADORES ===
generate_dto() {
  local name="$NAME"
  local dir="$ROOT_DIR/DTO/$name"
  local path="$dir/${name}DTO.cs"

  ensure_dir "$dir"

  local props
  props="$(extract_domain_properties || true)"
  if [[ -z "${props:-}" ]]; then
    warn "No se detectaron props en Domain/Entities/${name}.cs; usando fallback."
    props="$(fallback_dto_properties)"
  fi

  create_file "$path" "$(cat <<'EOF'
namespace DTO.__NAME__;

public class __NAME__DTO
{
__PROPS__
}
EOF
)"
  # Sustituciones
  # shellcheck disable=SC2016
  props_escaped="$(printf "%s" "$props")"
  sed -i \
    -e "s/__NAME__/$name/g" \
    "$path"
  # Inserta propiedades donde está __PROPS__
  awk -v repl="$props_escaped" '
    {
      if ($0 ~ /__PROPS__/) {
        gsub(/__PROPS__/, repl)
      }
      print
    }
  ' "$path" > "$path.tmp" && mv "$path.tmp" "$path"

  info "DTO generado: $(realpath --relative-to="$ROOT_DIR" "$path")"
}

generate_interface_application() {
  local name="$NAME"
  local dir="$ROOT_DIR/Interface/UseCases"
  local path="$dir/I${name}Application.cs"

  ensure_dir "$dir"

  create_file "$path" "$(cat <<'EOF'
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
EOF
)"
  sed -i "s/__NAME__/$name/g" "$path"
  info "Interface I${name}Application lista."
}

generate_usecase_application() {
  local name="$NAME"
  local plural="$PLURAL"
  local ns_plural="$PLURAL"
  local dir="$ROOT_DIR/UseCases/$ns_plural"
  local path="$dir/${name}Application.cs"

  ensure_dir "$dir"

  create_file "$path" "$(cat <<'EOF'
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

    public __NAME__Application(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        __NAME__DTOValidator validationRules,
        IAppLogger<__NAME__Application> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

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
                response.Message = "__NAME__ creado correctamente";
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
                response.Message = "__NAME__ modificado correctamente";
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
                response.Message = "__NAME__ eliminado correctamente";
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

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "__NAME__ encontrado";
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
            var list = _unitOfWork.__PLURAL__.GetAll();
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<__NAME__DTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<__NAME__DTO>>();
        try
        {
            var list = _unitOfWork.__PLURAL__.GetAllWithPagination(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(list);
            response.isSuccess = true;
            response.Page = page;
            response.PageSize = pageSize;
            response.Total = _unitOfWork.__PLURAL__.Count();
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
                response.Message = "__NAME__ creado correctamente";
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
                response.Message = "__NAME__ modificado correctamente";
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
                response.Message = "__NAME__ eliminado correctamente";
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

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "__NAME__ encontrado";
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
            var list = await _unitOfWork.__PLURAL__.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<__NAME__DTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<__NAME__DTO>>();
        try
        {
            var list = await _unitOfWork.__PLURAL__.GetAllWithPaginationAsync(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<__NAME__DTO>>(list);
            response.isSuccess = true;
            response.Page = page;
            response.PageSize = pageSize;
            response.Total = await _unitOfWork.__PLURAL__.CountAsync();
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
EOF
)"
  sed -i \
    -e "s/__NAME__/$name/g" \
    -e "s/__PLURAL__/$PLURAL/g" \
    "$path"

  info "UseCase ${name}Application listo."
}

ensure_mapping_profile() {
  local name="$NAME"
  local file="$ROOT_DIR/UseCases/Common/Mapping/MappingsProfile.cs"
  local newline="CreateMap<Domain.Entities.$name, DTO.$name.${name}DTO>().ReverseMap();"

  if [[ ! -f "$file" ]]; then
    warn "No existe $file; omitiendo inyección de mapping."
    return
  fi

  # Añade using DTO.<Entidad>; si no existe
  local using_ns="using DTO.$name;"
  if ! grep -q "^$using_ns\$" "$file"; then
    awk -v ins="$using_ns" '
      { lines[NR]=$0 }
      END {
        last_using=0
        for (i=1;i<=NR;i++) if (lines[i] ~ /^using[[:space:]].*;/) last_using=i
        for (i=1;i<=NR;i++) {
          print lines[i]
          if (i==last_using) print ins
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
    info "Agregado: $using_ns"
  fi

  # Inserta línea en el constructor
  awk -v mapline="$newline" '
    BEGIN{inserted=0; waitbrace=0}
    {
      print
      if (!inserted) {
        if ($0 ~ /public[[:space:]]+MappingsProfile\(\)[[:space:]]*\{/) {
          print "        " mapline
          inserted=1
        } else if ($0 ~ /public[[:space:]]+MappingsProfile\(\)/) {
          waitbrace=1
        } else if (waitbrace && $0 ~ /\{/) {
          print "        " mapline
          inserted=1
          waitbrace=0
        }
      }
    }
  ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

  if grep -qF "$newline" "$file"; then
    info "Mapping añadido en MappingsProfile.cs"
  else
    warn "No se pudo inyectar mapping; revisa el formato del constructor."
  fi
}

inject_dbset_into_dbcontext() {
  local name="$NAME"
  local plural="$PLURAL"
  local root="$ROOT_DIR"

  local file=""
  if [[ -f "$root/Persistence/Context/ApplicationDbContext.cs" ]]; then
    file="$root/Persistence/Context/ApplicationDbContext.cs"
  elif [[ -f "$root/Persistence/ApplicationDbContext.cs" ]]; then
    file="$root/Persistence/ApplicationDbContext.cs"
  else
    file="$(find "$root/Persistence" -type f -name '*DbContext.cs' | head -n 1 || true)"
  fi

  if [[ -z "$file" || ! -f "$file" ]]; then
    warn "No se encontró DbContext en Persistence; omitiendo DbSet."
    return
  fi

  if grep -q "DbSet<${name}>" "$file"; then
    info "DbSet<${name}> ya existe."
    return
  fi

  # using Domain.Entities;
  if ! grep -q '^using[[:space:]]\+Domain\.Entities;' "$file"; then
    awk '
      { lines[NR]=$0 }
      END {
        last_using=0
        for (i=1;i<=NR;i++) if (lines[i] ~ /^using[[:space:]].*;/) last_using=i
        for (i=1;i<=NR;i++) {
          print lines[i]
          if (i==last_using) print "using Domain.Entities;"
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
    info "Agregado: using Domain.Entities;"
  fi

  # Inserta la propiedad DbSet dentro de la clase DbContext (antes de la última llave)
  awk -v line="    public DbSet<${name}> ${plural} { get; set; }" '
    BEGIN{inserted=0}
    {
      if ($0 ~ /^\}/ && !inserted) {
        print "    " line
        inserted=1
      }
      print
    }
  ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

  info "DbSet<${name}> ${plural} inyectado en $(realpath --relative-to="$root" "$file")"
}

generate_repository_interface() {
  local name="$NAME"
  local dir="$ROOT_DIR/Interface/Persistence"
  local path="$dir/I${name}Repository.cs"

  ensure_dir "$dir"

  create_file "$path" "$(cat <<'EOF'
using Domain.Entities;

namespace Interface.Persistence;

public interface I__NAME__Repository
{
    #region Metodos sincronos
    bool Insert(__NAME__ entity);
    bool Update(__NAME__ entity);
    bool Delete(int id);
    __NAME__ Get(int id);
    IEnumerable<__NAME__> GetAll();
    IEnumerable<__NAME__> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(__NAME__ entity);
    Task<bool> UpdateAsync(__NAME__ entity);
    Task<bool> DeleteAsync(int id);
    Task<__NAME__> GetAsync(int id);
    Task<IEnumerable<__NAME__>> GetAllAsync();
    Task<IEnumerable<__NAME__>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}
EOF
)"
  sed -i "s/__NAME__/$name/g" "$path"
  info "Interface repo I${name}Repository lista."
}

generate_repository_impl() {
  local name="$NAME"        # p.ej. CatalogItem
  local plural="$PLURAL"    # p.ej. CatalogItems (DbSet y propiedad pública)
  local dir="$ROOT_DIR/Persistence/Repositories"
  local path="$dir/${name}Repository.cs"

  ensure_dir "$dir"

  create_file "$path" "$(cat <<'EOF'
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class __NAME__Repository : I__NAME__Repository
{
    protected readonly ApplicationDbContext _context;

    public __NAME__Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(__NAME__ entity)
    {
        _context.__DBSET__.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(__NAME__ entity)
    {
        _context.__DBSET__.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.__DBSET__.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public __NAME__ Get(int id)
    {
        return _context.__DBSET__.Find(id);
    }

    public IEnumerable<__NAME__> GetAll()
    {
        return _context.__DBSET__;
    }

    public IEnumerable<__NAME__> GetAllWithPagination(int page, int pageSize)
    {
        return _context.__DBSET__
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.__DBSET__.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(__NAME__ entity)
    {
        await _context.__DBSET__.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(__NAME__ entity)
    {
        _context.__DBSET__.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.__DBSET__.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<__NAME__> GetAsync(int id)
    {
        return await _context.__DBSET__.FindAsync(id);
    }

    public async Task<IEnumerable<__NAME__>> GetAllAsync()
    {
        return await _context.__DBSET__.ToListAsync();
    }

    public async Task<IEnumerable<__NAME__>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.__DBSET__
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.__DBSET__.CountAsync();
    }

    #endregion
}
EOF
)"
  sed -i \
    -e "s/__NAME__/$name/g" \
    -e "s/__DBSET__/$plural/g" \
    "$path"

  info "Repository impl creada: $(realpath --relative-to="$ROOT_DIR" "$path")"
}

inject_repository_into_unitofwork_interface() {
  local name="$NAME"
  local plural="$PLURAL"
  local file="$ROOT_DIR/Interface/Persistence/IUnitOfWork.cs"

  if [[ ! -f "$file" ]]; then
    warn "No existe IUnitOfWork.cs; omitiendo inyección (interface)."
    return
  fi

  # using Interface.Persistence;
  if ! grep -q "^using[[:space:]]\+Interface\.Persistence;" "$file"; then
    awk '
      { lines[NR]=$0 }
      END {
        last_using=0
        for (i=1;i<=NR;i++) if (lines[i] ~ /^using[[:space:]].*;/) last_using=i
        for (i=1;i<=NR;i++) {
          print lines[i]
          if (i==last_using) print "using Interface.Persistence;"
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi

  # Inserta propiedad si no existe
  if grep -q "I${name}Repository[[:space:]]\+${plural}[[:space:]]*{[[:space:]]*get;" "$file"; then
    info "IUnitOfWork ya expone ${plural}."
  else
    awk -v newline="    I${name}Repository ${plural} { get; }" '
      BEGIN{inserted=0}
      {
        if ($0 ~ /^\}/ && !inserted) {
          print newline
          inserted=1
        }
        print
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
    info "IUnitOfWork: agregada propiedad I${name}Repository ${plural} { get; }"
  fi
}

inject_repository_into_unitofwork_impl() {
  local name="$NAME"
  local plural="$PLURAL"
  local root="$ROOT_DIR"
  local file="$root/Persistence/Repositories/UnitOfWork.cs"

  if [[ ! -f "$file" ]]; then
    warn "No existe Persistence/Repositories/UnitOfWork.cs; omitiendo inyección (impl)."
    return
  fi

  # using Interface.Persistence;
  if ! grep -q "^using[[:space:]]\+Interface\.Persistence;" "$file"; then
    awk '
      { lines[NR]=$0 }
      END {
        last_using=0
        for (i=1;i<=NR;i++) if (lines[i] ~ /^using[[:space:]].*;/) last_using=i
        for (i=1;i<=NR;i++) {
          print lines[i]
          if (i==last_using) print "using Interface.Persistence;"
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
    info "Agregado using Interface.Persistence; en UnitOfWork.cs"
  fi

  # Propiedad pública si no existe
  if grep -q "public[[:space:]]\+I${name}Repository[[:space:]]\+${plural}[[:space:]]*{[[:space:]]*get;" "$file"; then
    info "UnitOfWork ya tiene propiedad ${plural}."
  else
    awk -v newline="    public I${name}Repository ${plural} { get; }" '
      { print }
      /private readonly ApplicationDbContext _context;|private readonly .*_context;/ && !done {
        # mantenemos después del campo _context;
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

    # Si no pudo inyectar en lugar específico, añádelo antes del constructor
    if ! grep -q "public[[:space:]]\+I${name}Repository[[:space:]]\+${plural}[[:space:]]*{[[:space:]]*get;" "$file"; then
      awk -v newline="    public I${name}Repository ${plural} { get; }" '
        BEGIN{done=0}
        {
          if (!done && $0 ~ /public[[:space:]]+UnitOfWork\(/) {
            print newline
            done=1
          }
          print
        }
      ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
    fi
    info "UnitOfWork: agregada propiedad ${plural}."
  fi

  # Inyección por constructor: añade parámetro I<Name>Repository <camelRepo>
  local camelRepo
  camelRepo="$(echo "${name}Repository" | sed -E 's/^([A-Z])/\L\1/')" # e.g. catalogItemRepository
  if grep -q "${camelRepo}" "$file"; then
    info "UnitOfWork: parece que ya recibe ${camelRepo}."
  else
    # Añadir parámetro y asignación básica
    awk -v name="$name" -v camel="$camelRepo" -v plural="$plural" '
      BEGIN{inCtor=0; addedParam=0; assigned=0}
      {
        line=$0
        if ($0 ~ /public[[:space:]]+UnitOfWork\(/) inCtor=1
        if (inCtor && !addedParam && $0 ~ /\)\s*\{/ ) {
          sub(/\)\s*\{/, ", I"name"Repository "camel") 
          print
          inCtor=0; addedParam=1
          next
        }
        print
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

    # Asignación de propiedad dentro del constructor
    awk -v plural="$plural" -v camel="$camelRepo" '
      BEGIN{inCtor=0; done=0}
      {
        print
        if ($0 ~ /public[[:space:]]+UnitOfWork\(/) inCtor=1
        else if (inCtor && $0 ~ /\{/) { 
          if (!done) {
            print "        " plural " = " camel ";"
            done=1
          }
        } else if (inCtor && $0 ~ /\}/) {
          inCtor=0
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

    info "UnitOfWork: inyectado parámetro y asignación para ${plural}."
  fi
}

# === EJECUCIÓN ===
generate_dto
generate_interface_application
generate_usecase_application
ensure_mapping_profile
inject_dbset_into_dbcontext
generate_repository_interface
generate_repository_impl
inject_repository_into_unitofwork_interface
inject_repository_into_unitofwork_impl

info "Listo. Revisa cambios y compila."
