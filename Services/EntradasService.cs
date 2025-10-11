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

        var entradaExistente = await contexto.EntradasHuacales
        .Include(e => e.EntradasHuacalesDetalles)
        .FirstOrDefaultAsync(e => e.IdEntrada == entrada.IdEntrada);

        if (entradaExistente == null)
        {
            return false;
        }

        // 2. Afectar existencias (restar los detalles anteriores)
        await AfectarEntradas(entradaExistente.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Resta);

        // 3. Actualizar la entrada principal
        contexto.Entry(entradaExistente).CurrentValues.SetValues(entrada);

        // 4. Actualizar los detalles
        // Eliminar detalles que ya no existen
        foreach (var detalleExistente in entradaExistente.EntradasHuacalesDetalles.ToList())
        {
            if (!entrada.EntradasHuacalesDetalles.Any(d => d.DetalleId == detalleExistente.DetalleId))
            {
                contexto.Remove(detalleExistente);
            }
        }

        // Actualizar/agregar detalles
        foreach (var detalle in entrada.EntradasHuacalesDetalles)
        {
            var detalleExistente = entradaExistente.EntradasHuacalesDetalles
                .FirstOrDefault(d => d.DetalleId == detalle.DetalleId);

            if (detalleExistente != null)
            {
                // Actualizar detalle existente
                contexto.Entry(detalleExistente).CurrentValues.SetValues(detalle);
            }
            else
            {
                // Agregar nuevo detalle
                detalle.DetalleId = 0; // Para que sea auto-generado
                entradaExistente.EntradasHuacalesDetalles.Add(detalle);
            }
        }

        // 5. Afectar existencias (sumar los nuevos detalles)
        await AfectarEntradas(entrada.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Suma);

        // 6. Guardar TODOS los cambios de una vez
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarEntradas(EntradasHuacalesDetalle[] detalles, TipoOperacion tipoOperacion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var tipoIds = detalles.Select(d => d.TipoId).Distinct();
        var tiposHuacal = await contexto.TiposHuacales
            .Where(t => tipoIds.Contains(t.TipoId))
            .ToListAsync();

        foreach (var detalle in detalles)
        {
            var tipoHuacal = tiposHuacal.FirstOrDefault(t => t.TipoId == detalle.TipoId);
            if (tipoHuacal != null)
            {
                if (tipoOperacion == TipoOperacion.Suma)
                {
                    tipoHuacal.Existencia += detalle.Cantidad;
                }
                else if (tipoOperacion == TipoOperacion.Resta)
                {
                    tipoHuacal.Existencia -= detalle.Cantidad;
                    if (tipoHuacal.Existencia < 0)
                    {
                        tipoHuacal.Existencia = 0;
                    }
                }
            }
        }

        await contexto.SaveChangesAsync();
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
