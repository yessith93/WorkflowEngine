using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface ISecuenciaRepository : IRepository<Secuencia>
    {
        Task<Secuencia?> GetWithPasosAsync(int id);
        Task<IEnumerable<Secuencia>> GetByTipoFlujoAsync(int tipoFlujoId);
        Task<List<int>> GetPasosIdsAsync(int secuenciaId);
    }
}
