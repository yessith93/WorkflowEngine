using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;

namespace WorkflowEngine.Application.Interfaces.UseCases
{
    public interface IProcesarFlujoUseCase
    {
        Task<ResultadoProcesarFlujo> ExecuteAsync(int idFlujoActivo, Dictionary<int, string> datosFormulario);
    }
}
