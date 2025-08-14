using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.Interfaces.UseCases
{
    public interface IIdentificarSiguienteSecuenciaUseCase
    {
        Task<int?> ExecuteAsync(int idFlujoActivo);
    }
}
