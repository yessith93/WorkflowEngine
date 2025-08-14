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
    public class PasoRepository : Repository<Paso>, IPasoRepository
    {
        public PasoRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<Paso?> GetWithCamposAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Campos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Paso>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbSet
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<List<int>> GetCamposIdsAsync(int pasoId)
        {
            var paso = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == pasoId);

            if (paso == null || string.IsNullOrEmpty(paso.ListaIdCampos))
                return new List<int>();

            return paso.ListaIdCampos
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }

        public async Task<IEnumerable<Paso>> GetByIdsWithCamposAsync(IEnumerable<int> pasoIds)
        {
            return await _dbSet
                .Where(p => pasoIds.Contains(p.Id))
                .Include(p => p.Campos)
                .ToListAsync();
        }
    }
}
