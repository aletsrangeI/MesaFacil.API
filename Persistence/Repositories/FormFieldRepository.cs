using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

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
        return _context.FormFields;
    }

    public IEnumerable<FormField> GetAllWithPagination(int page, int pageSize)
    {
        return _context.FormFields
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.FormFields.Count();
    }

    public IEnumerable<FormField> GetFormFieldByFormCatId(int id)
    {
        var result = (from ff in _context.FormFields
            join cc in _context.CatalogItems
                on ff.FormularioItemId equals cc.Id
            where ff.FormularioItemId == id
            select ff).OrderBy(a => a.Order).ToList();

        foreach (var formField in result)
        {
            if (formField.Type == "select" && formField.CatalogId.HasValue)
            {
                formField.Options = FillOptions(formField.Id, formField.CatalogId.Value);
            }
        }

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
        return await _context.FormFields.ToListAsync();
    }

    public async Task<IEnumerable<FormField>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.FormFields
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.FormFields.CountAsync();
    }

    public async Task<IEnumerable<FormField>> GetFormFieldByFormCatIdAsync(int id)
    {
        var result = await (from ff in _context.FormFields
            join cc in _context.CatalogItems
                on ff.FormularioCatalogId equals cc.Id
            where ff.FormularioCatalogId == id
            select ff).OrderBy(a => a.Order).ToListAsync();

        foreach (var formField in result)
        {
            if (formField.Type == "select" && formField.CatalogId.HasValue)
            {
                formField.Options = await FillOptionsAsync(formField.Id, formField.CatalogId.Value);
            }
        }

        return result;
    }

    private List<SelectFormOption> FillOptions(int formFieldId, int catalogoId)
    {
        List<SelectFormOption> options = _context.CatalogItems
            .Where(cc => cc.CatalogId == catalogoId)
            .Select(cc => new SelectFormOption
            {
                Id = cc.Id,
                Nombre = cc.Name
            }).ToList();
        return options;
    }

    private async Task<List<SelectFormOption>> FillOptionsAsync(int formFieldId, int catalogoId)
    {
        List<SelectFormOption> options = await _context.CatalogItems
            .Where(cc => cc.CatalogId == catalogoId)
            .Select(cc => new SelectFormOption
            {
                Id = cc.Id,
                Nombre = cc.Name
            }).ToListAsync();
        return options;
    }

    #endregion
}