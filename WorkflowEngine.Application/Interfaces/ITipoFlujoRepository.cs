using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Interfaces
{
    public interface ITipoFlujoRepository : IRepository<TipoFlujo>
    {
        Task<TipoFlujo?> GetWithSecuenciasAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
