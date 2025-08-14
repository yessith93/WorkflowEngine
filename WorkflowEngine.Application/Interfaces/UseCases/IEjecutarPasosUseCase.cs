using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.Interfaces.UseCases
{
    public interface IEjecutarPasosUseCase
    {
        Task ExecuteAsync(List<int> idsPasos, Dictionary<int, string> datosPreparados);
    }
}
