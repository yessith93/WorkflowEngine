using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IPasoRepository : IRepository<Paso>
    {
        Task<Paso?> GetWithCamposAsync(int id);
        Task<IEnumerable<Paso>> GetByIdsAsync(IEnumerable<int> ids);
        Task<List<int>> GetCamposIdsAsync(int pasoId);

    }
}
