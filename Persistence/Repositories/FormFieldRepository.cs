using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class FormFieldRepository : IFormFieldRepository
{
    protected readonly ApplicationDbContext _context;

    public FormFieldRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(FormField entity)
    {
        _context.FormFields.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(FormField entity)
    {
        _context.FormFields.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.FormFields.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public FormField Get(int id)
    {
        return _context.FormFields.Find(id);
    }

    public IEnumerable<FormField> GetAll()
    {
        return _context.FormFields.AsNoTracking().ToList();
    }

    public IEnumerable<FormField> GetAllWithPagination(int page, int pageSize)
    {
        return _context.FormFields
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.FormFields.Count();
    }

    // [CORREGIDO] Ahora busca por el Código del Formulario directamente
    public IEnumerable<FormField> GetFormFieldByFormCode(string code)
    {
        var result = _context.FormFields
            .Include(ff => ff.Formulario)
            .Where(ff => ff.Formulario.Codigo == code && ff.IsActive)
            .OrderBy(a => a.Orden)
            .ToList();

        return result;
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(FormField entity)
    {
        await _context.FormFields.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(FormField entity)
    {
        _context.FormFields.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.FormFields.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<FormField> GetAsync(int id)
    {
        return await _context.FormFields.FindAsync(id);
    }

    public async Task<IEnumerable<FormField>> GetAllAsync()
    {
        return await _context.FormFields.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<FormField>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.FormFields
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.FormFields.CountAsync();
    }

    // [CORREGIDO] Búsqueda asíncrona por ID de Formulario (o Código)
    public async Task<IEnumerable<FormField>> GetFormFieldByFormIdAsync(int formularioId)
    {
        return await _context.FormFields
            .Where(ff => ff.IdFormulario == formularioId && ff.IsActive)
            .OrderBy(a => a.Orden)
            .ToListAsync();
    }

    #endregion
}