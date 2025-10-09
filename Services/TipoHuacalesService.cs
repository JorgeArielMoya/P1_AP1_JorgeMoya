using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P1_AP1_JorgeMoya.DAL;
using P1_AP1_JorgeMoya.Models;

namespace P1_AP1_JorgeMoya.Services;

public class TipoHuacalesService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<TiposHuacales?> Buscar (int tipoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposHuacales.FirstOrDefaultAsync(t => t.TipoId == tipoId);
    }

    public async Task<List<TiposHuacales>> Listar(Expression<Func<TiposHuacales, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposHuacales
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
