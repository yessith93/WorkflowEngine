using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface ICampoRepository : IRepository<Campo>
    {
        Task<IEnumerable<Campo>> GetByIdsAsync(IEnumerable<int> ids);
        Task<Campo?> GetWithValidationAsync(int id);
    }
}
