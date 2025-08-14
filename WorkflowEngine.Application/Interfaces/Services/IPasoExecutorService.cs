using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.Interfaces.Services
{
    public interface IPasoExecutorService
    {
        Task ExecuteAsync(List<int> idsPasos, Dictionary<int, string> datos);
    }
}
