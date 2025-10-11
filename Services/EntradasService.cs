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

    private async Task<bool> Modificar(EntradasHuacales entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entradaExistente = await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
            .FirstOrDefaultAsync(e => e.IdEntrada == entrada.IdEntrada);

        if (entradaExistente == null)
        {
            return false;
        }

        await AfectarEntradas(entradaExistente.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Resta);

        contexto.EntradasHuacalesDetalles.RemoveRange(entradaExistente.EntradasHuacalesDetalles);

        entradaExistente.NombreCliente = entrada.NombreCliente;
        entradaExistente.Fecha = entrada.Fecha;

        foreach (var detalle in entrada.EntradasHuacalesDetalles)
        {
            var nuevoDetalle = new EntradasHuacalesDetalle
            {
                TipoId = detalle.TipoId,
                Cantidad = detalle.Cantidad,
                Precio = detalle.Precio
            };
            entradaExistente.EntradasHuacalesDetalles.Add(nuevoDetalle);
        }
        await AfectarEntradas(entrada.EntradasHuacalesDetalles.ToArray(), TipoOperacion.Suma);

        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarEntradas(ICollection<EntradasHuacalesDetalle> detalle, TipoOperacion tipoOperacion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        foreach (var item in detalle)
        {
            var tipoHuacal = await contexto.TiposHuacales
                .SingleAsync(t => t.TipoId == item.TipoId);

            var cantidadEntrada = item.Cantidad;

            if (tipoOperacion == TipoOperacion.Suma)
            {
                tipoHuacal.Existencia += cantidadEntrada;
            }
            else
            {
                tipoHuacal.Existencia -= cantidadEntrada;

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

        var entidad = await contexto.EntradasHuacales
            .Include(e => e.EntradasHuacalesDetalles)
            .FirstOrDefaultAsync(e => e.IdEntrada == entradaId);

        if (entidad is null) return false;

        await AfectarEntradas(entidad.EntradasHuacalesDetalles, TipoOperacion.Resta);

        contexto.EntradasHuacalesDetalles.RemoveRange(entidad.EntradasHuacalesDetalles);
        contexto.EntradasHuacales.Remove(entidad);
        return await contexto.SaveChangesAsync() > 0;
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
