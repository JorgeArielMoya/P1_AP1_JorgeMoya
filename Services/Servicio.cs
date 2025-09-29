using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P1_AP1_JorgeMoya.DAL;
using P1_AP1_JorgeMoya.Models;

namespace P1_AP1_JorgeMoya.Services;

public class Servicio(IDbContextFactory<Contexto>DbFactory)
{
    public async Task<bool> Guardar (EntradasHuacales entrada)
    {
        if (!await Existe (entrada.IdEntrada))
        {
            return await Insertar (entrada);    
        }

        else
        {
            return await Modificar (entrada); 
        }
    }

    public async Task<bool> Existe (int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales.AnyAsync(e=> e.IdEntrada ==  entradaId); 
    }

    private async Task<bool> Insertar (EntradasHuacales entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.EntradasHuacales.Add(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar (EntradasHuacales entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<EntradasHuacales?> Buscar (int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales.FirstOrDefaultAsync(e => e.IdEntrada == entradaId);
    }

    public async Task<bool> Eliminar (int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales
            .AsNoTracking()
            .Where(e=> e.IdEntrada == entradaId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<EntradasHuacales>> Listar (Expression<Func<EntradasHuacales,bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
