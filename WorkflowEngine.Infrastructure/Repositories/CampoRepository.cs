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
    public class CampoRepository : Repository<Campo>, ICampoRepository
    {
        public CampoRepository(WorkflowContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Campo>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbSet
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<Campo?> GetWithValidationAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
