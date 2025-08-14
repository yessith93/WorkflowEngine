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
    public class TipoFlujoRepository : Repository<TipoFlujo>, ITipoFlujoRepository
    {
        public TipoFlujoRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<TipoFlujo?> GetWithSecuenciasAsync(int id)
        {
            return await _dbSet
                .Include(tf => tf.Secuencias)
                .FirstOrDefaultAsync(tf => tf.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(tf => tf.Id == id);
        }
    }
}
