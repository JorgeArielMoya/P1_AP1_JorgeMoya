using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P1_AP1_JorgeMoya.DAL;
using P1_AP1_JorgeMoya.Models;

namespace P1_AP1_JorgeMoya.Services;

public class EntradasService(IDbContextFactory<Contexto>DbFactory)
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
        await AfectarEntradas(entrada.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Suma);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar (EntradasHuacales entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entradaAnterior = await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdEntrada == entrada.IdEntrada);

        if (entradaAnterior == null)
        {
            return false;
        }

        await AfectarEntradas(entradaAnterior.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Resta);

        await AfectarEntradas(entrada.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Suma);

        contexto.EntradasHuacales.Update(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarEntradas(EntradasHuacalesDetalle[] detalle, TipoOperacion tipoOperacion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        foreach (var entrada in detalle)
        {
            var tipoHuacal = await contexto.TiposHuacales
                .SingleAsync(t => t.TipoId == entrada.TipoId);

            if (tipoOperacion == TipoOperacion.Suma)
            {
                tipoHuacal.Existencia += entrada.Cantidad;
            }

            else if (tipoOperacion == TipoOperacion.Resta)
            {
                tipoHuacal.Existencia -= entrada.Cantidad;

                if (tipoHuacal.Existencia < 0)
                {
                    tipoHuacal.Existencia = 0;
                }
            }

            await contexto.SaveChangesAsync();
        }
    }

    public async Task<EntradasHuacales?> Buscar (int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
                .ThenInclude(d => d.TiposHuacales)
            .FirstOrDefaultAsync(e => e.IdEntrada == entradaId);
    }

    public async Task<bool> Eliminar (int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entrada = await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
            .FirstOrDefaultAsync(e => e.IdEntrada == entradaId);

        if (entrada == null)
        {
            return false;
        }

        await AfectarEntradas(entrada.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Resta);

        contexto.EntradasHuacalesDetalles.RemoveRange(entrada.EntradasHuacalesDetalles);
        contexto.EntradasHuacales.Remove(entrada);

        var cantidad = await contexto.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<List<EntradasHuacales>> Listar (Expression<Func<EntradasHuacales,bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<TiposHuacales>> ListarTiposHuacales()
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposHuacales
            .AsNoTracking()
            .ToListAsync();
    }

    public enum TipoOperacion
    {
        Suma = 1,
        Resta = 2
    }
}
