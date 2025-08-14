using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IFlujoActivoRepository : IRepository<FlujoActivo>
    {
        Task<FlujoActivo?> GetWithTipoFlujoAsync(int id);
        Task<FlujoActivo?> GetWithCamposAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task UpdateEstadoAsync(int id, EstadoFlujo nuevoEstado);
        Task UpdateSecuenciaActualAsync(int id, int nuevaSecuenciaId);
    }
}
