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
    public class SecuenciaRepository : Repository<Secuencia>, ISecuenciaRepository
    {
        public SecuenciaRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<Secuencia?> GetWithPasosAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Pasos)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Secuencia>> GetByTipoFlujoAsync(int tipoFlujoId)
        {
            return await _dbSet
                .Where(s => s.IdTipoFlujo == tipoFlujoId)
                .ToListAsync();
        }

        public async Task<List<int>> GetPasosIdsAsync(int secuenciaId)
        {
            var secuencia = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == secuenciaId);

            if (secuencia == null || string.IsNullOrEmpty(secuencia.ListaIdPasos))
                return new List<int>();

            return secuencia.ListaIdPasos
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }
    }
}
