using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface ICampoFlujoActivoRepository : IRepository<CampoFlujoActivo>
    {
        Task<IEnumerable<CampoFlujoActivo>> GetByFlujoActivoAsync(int flujoActivoId);
        Task<CampoFlujoActivo?> GetByFlujoAndCampoAsync(int flujoActivoId, int campoId);
        Task<Dictionary<int, string>> GetCamposValoresByFlujoAsync(int flujoActivoId);
        Task<List<int>> GetCamposExistentesAsync(int flujoActivoId, IEnumerable<int> camposIds);
        Task UpsertCampoAsync(int flujoActivoId, int campoId, string valor);
    }
}
