using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;

namespace WorkflowEngine.Application.Interfaces.UseCases
{
    public interface IPrepararSecuenciaUseCase
    {
        Task<ResultadoPrepararSecuencia> ExecuteAsync(int idSecuencia, Dictionary<int, string> datosRecibidos, int idFlujoActivo);
    }
}
