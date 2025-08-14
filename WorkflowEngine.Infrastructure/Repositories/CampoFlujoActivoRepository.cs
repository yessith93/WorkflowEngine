using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Infrastructure.Data;

namespace WorkflowEngine.Infrastructure.Repositories
{
    public class CampoFlujoActivoRepository : Repository<CampoFlujoActivo>, ICampoFlujoActivoRepository
    {
        public CampoFlujoActivoRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CampoFlujoActivo>> GetByFlujoActivoAsync(int flujoActivoId)
        {
            return await _dbSet
                .Where(cfa => cfa.IdFlujoActivo == flujoActivoId)
                .Include(cfa => cfa.Campo)
                .ToListAsync();
        }

        public async Task<CampoFlujoActivo?> GetByFlujoAndCampoAsync(int flujoActivoId, int campoId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(cfa => cfa.IdFlujoActivo == flujoActivoId && cfa.IdCampo == campoId);
        }

        public async Task<Dictionary<int, string>> GetCamposValoresByFlujoAsync(int flujoActivoId)
        {
            return await _dbSet
                .Where(cfa => cfa.IdFlujoActivo == flujoActivoId)
                .ToDictionaryAsync(cfa => cfa.IdCampo, cfa => cfa.Valor);
        }

        public async Task<List<int>> GetCamposExistentesAsync(int flujoActivoId, IEnumerable<int> camposIds)
        {
            return await _dbSet
                .Where(cfa => cfa.IdFlujoActivo == flujoActivoId && camposIds.Contains(cfa.IdCampo))
                .Select(cfa => cfa.IdCampo)
                .ToListAsync();
        }

        public async Task UpsertCampoAsync(int flujoActivoId, int campoId, string valor)
        {
            var existente = await GetByFlujoAndCampoAsync(flujoActivoId, campoId);

            if (existente != null)
            {
                existente.Valor = valor;
                Update(existente);
            }
            else
            {
                await AddAsync(new CampoFlujoActivo
                {
                    IdFlujoActivo = flujoActivoId,
                    IdCampo = campoId,
                    Valor = valor
                });
            }
        }
    }
}
