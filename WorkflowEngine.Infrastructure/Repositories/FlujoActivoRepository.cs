using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;
using WorkflowEngine.Infrastructure.Data;

namespace WorkflowEngine.Infrastructure.Repositories
{
    public class FlujoActivoRepository : Repository<FlujoActivo>, IFlujoActivoRepository
    {
        public FlujoActivoRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<FlujoActivo?> GetWithTipoFlujoAsync(int id)
        {
            return await _dbSet
                .Include(fa => fa.TipoFlujo)
                .FirstOrDefaultAsync(fa => fa.Id == id);
        }

        public async Task<FlujoActivo?> GetWithCamposAsync(int id)
        {
            return await _dbSet
                .Include(fa => fa.CamposFlujosActivos)
                    .ThenInclude(cfa => cfa.Campo)
                .FirstOrDefaultAsync(fa => fa.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(fa => fa.Id == id);
        }

        public async Task UpdateEstadoAsync(int id, EstadoFlujo nuevoEstado)
        {
            var flujo = await _dbSet.FirstOrDefaultAsync(fa => fa.Id == id);
            if (flujo != null)
            {
                flujo.EstadoFlujo = nuevoEstado;
                if (nuevoEstado == EstadoFlujo.Completado)
                {
                    flujo.FechaCompletado = DateTime.UtcNow;
                }
            }
        }

        public async Task UpdateSecuenciaActualAsync(int id, int nuevaSecuenciaId)
        {
            var flujo = await _dbSet.FirstOrDefaultAsync(fa => fa.Id == id);
            if (flujo != null)
            {
                flujo.IdSecuenciaActual = nuevaSecuenciaId;
            }
        }
    }
}
