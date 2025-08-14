using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITipoFlujoRepository TiposFlujo { get; }
        ISecuenciaRepository Secuencias { get; }
        IPasoRepository Pasos { get; }
        ICampoRepository Campos { get; }
        IFlujoActivoRepository FlujosActivos { get; }
        ICampoFlujoActivoRepository CamposFlujosActivos { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
