using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.Interfaces.Services
{
    public interface IPasoHandler
    {
        Task<bool> CanHandle(Domain.Enums.PasoTipo tipoPaso);
        Task ExecuteAsync(Dictionary<int, string> datos);
    }
}
